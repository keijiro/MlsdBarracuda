using UnityEngine;

namespace Mlsd {

public sealed class StaticImageTest : MonoBehaviour
{
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Texture2D _image = null;
    [SerializeField] Shader _shader = null;

    LineSegmentDetector _detector;
    Material _material;

    void Start()
    {
        _detector = new LineSegmentDetector(_resources);
        _detector.ProcessImage(_image);

        _material = new Material(_shader);
        _material.SetTexture("_LineData", _detector.BakedTexture);
    }

    void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);
    }

    void Update()
      => Graphics.DrawProcedural
           (_material, new Bounds(Vector3.zero, Vector3.one * 1000),
            MeshTopology.Lines, 2, LineSegmentDetector.MaxDetection);
}

} // namespace Mlsd
