using UnityEngine;
using Unity.Barracuda;

namespace Mlsd {

public sealed class LineSegmentDetector : System.IDisposable
{
    #region Public members

    public const int MaxDetection = 4096;

    public RenderTexture BakedTexture => _buffers.output;

    public LineSegmentDetector(ResourceSet resources)
    {
        _resources = resources;
        AllocateObjects();
    }

    public void Dispose()
      => DeallocateObjects();

    public void ProcessImage(Texture sourceTexture, float threshold = 0.1f)
      => RunModel(sourceTexture, threshold);

    #endregion

    #region Private objects

    ResourceSet _resources;
    (int w, int h) _size;
    IWorker _worker;

    (ComputeBuffer preprocess,
     RenderTexture scores,
     RenderTexture offsets,
     RenderTexture output,
     ComputeBuffer counter) _buffers;

    void AllocateObjects()
    {
        var model = ModelLoader.Load(_resources.model);

        var shape = model.inputs[0].shape; // NHWC
        _size = (shape[6], shape[5]);      // (W, H)

        _worker = model.CreateWorker();

        _buffers = (new ComputeBuffer(_size.w * _size.h * 4, sizeof(float)),
                    RTUtil.NewFloat(_size.w, _size.h),
                    RTUtil.NewFloat4(_size.w, _size.h),
                    RTUtil.NewFloat4UAV(MaxDetection, 1),
                    new ComputeBuffer(1, 4, ComputeBufferType.Counter));
    }


    void DeallocateObjects()
    {
        _worker?.Dispose();
        _worker = null;

        _buffers.preprocess?.Dispose();
        Object.Destroy(_buffers.scores);
        Object.Destroy(_buffers.offsets);
        Object.Destroy(_buffers.output);
        _buffers.counter?.Dispose();
        _buffers = (null, null, null, null, null);
    }

    #endregion

    #region Main inference function

    void RunModel(Texture source, float threshold)
    {
        // Preprocessing
        var pre = _resources.preprocess;
        pre.SetInts("Size", _size.w, _size.h);
        pre.SetTexture(0, "Image", source);
        pre.SetBuffer(0, "Tensor", _buffers.preprocess);
        pre.DispatchThreads(0, _size.w, _size.h, 1);

        // NN worker invocation
        using (var tensor = new Tensor(1, _size.h, _size.w, 4,
                                       _buffers.preprocess))
            _worker.Execute(tensor);

        // Postprocessing
        _worker.PeekOutput("scores").ToRenderTexture(_buffers.scores);
        _worker.PeekOutput("offsets").ToRenderTexture(_buffers.offsets);

        _buffers.counter.SetCounterValue(0);

        var post = _resources.postprocess;
        post.SetInts("Size", _size.w, _size.h);
        post.SetFloat("Threshold", threshold);
        post.SetTexture(0, "Scores", _buffers.scores);
        post.SetTexture(0, "Offsets", _buffers.offsets);
        post.SetTexture(0, "Output", _buffers.output);
        post.SetBuffer(0, "Counter", _buffers.counter);
        post.SetInt("MaxDetection", MaxDetection);
        post.DispatchThreads(0, _size.w, _size.h, 1);

        post.SetTexture(1, "Output", _buffers.output);
        post.SetBuffer(1, "Counter", _buffers.counter);
        post.Dispatch(1, 1, 1, 1);
    }

    #endregion
}

} // namespace Mlsd
