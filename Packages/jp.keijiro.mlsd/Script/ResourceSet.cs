using UnityEngine;
using Unity.Barracuda;

namespace Mlsd {

[CreateAssetMenu(fileName = "MLSD",
                 menuName = "ScriptableObjects/MLSD Resource Set")]
public sealed class ResourceSet : ScriptableObject
{
    public NNModel model;
    public ComputeShader preprocess;
    public ComputeShader postprocess;
}

} // namespace Mlsd
