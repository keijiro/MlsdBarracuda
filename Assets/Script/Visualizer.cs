using UnityEngine;
using UI = UnityEngine.UI;
using Mlsd;

public sealed class Visualizer : MonoBehaviour
{
    [SerializeField] WebcamInput _webcam = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField, Range(0.001f, 1)] float _threshold = 0.1f;
    [SerializeField] UI.RawImage _previewUI = null;
    [SerializeField] Shader _shader = null;

    LineSegmentDetector _detector;
    Material _material;

    void Start()
    {
        _detector = new LineSegmentDetector(_resources);
        _material = new Material(_shader);
    }

    void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);
    }

    void Update()
    {
        _detector.ProcessImage(_webcam.Texture, _threshold);

        _material.SetTexture("_LineData", _detector.BakedTexture);

        Graphics.DrawProcedural
          (_material, new Bounds(Vector3.zero, Vector3.one * 1000),
           MeshTopology.Lines, 2, LineSegmentDetector.MaxDetection);

        _previewUI.texture = _webcam.Texture;
    }
}
