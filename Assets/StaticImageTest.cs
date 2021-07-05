using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Barracuda;

namespace Mlsd {

public sealed class StaticImageTest : MonoBehaviour
{
    [SerializeField] NNModel _model;
    [SerializeField] Texture2D _image;

    void Start()
    {
        // Input image -> Tensor (1, 320, 320, 4)
        var source = new float[320 * 320 * 4];

        for (var y = 0; y < 320; y++)
        {
            for (var x = 0; x < 320; x++)
            {
                var i = (y * 320 + x) * 4;
                var p = _image.GetPixel(x, 320 - y);
                source[i + 0] = p.r * 255;
                source[i + 1] = p.g * 255;
                source[i + 2] = p.b * 255;
                source[i + 3] = 255;
            }
        }

        // Inference
        var model = ModelLoader.Load(_model);
        using var worker = WorkerFactory.CreateWorker(model);

        using (var tensor = new Tensor(1, 320, 320, 4, source))
            worker.Execute(tensor);

        // Visualization
        var offsets = worker.PeekOutput("offsets");
        var scores = worker.PeekOutput("scores");

        var vtx = new List<Vector3>();

        for (var y = 0; y < 160; y++)
        {
            for (var x = 0; x < 160; x++)
            {
                var score = scores[0, y, x, 0];
                if (score < 0.1f) continue;

                var x1 = x + offsets[0, y, x, 0];
                var y1 = y + offsets[0, y, x, 1];
                var x2 = x + offsets[0, y, x, 2];
                var y2 = y + offsets[0, y, x, 3];

                vtx.Add(new Vector3(x1, y1, 0));
                vtx.Add(new Vector3(x2, y2, 0));
            }
        }

        var mesh = new Mesh();
        mesh.SetVertices(vtx);
        mesh.SetIndices(Enumerable.Range(0, vtx.Count).ToArray(), MeshTopology.Lines, 0);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}

} // namespace Mlsd
