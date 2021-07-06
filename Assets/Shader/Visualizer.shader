Shader "Hidden/MLSD/Visualizer"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    float4 _LineColor;
    Texture2D<float4> _LineData;

    void Vertex(uint vid : SV_VertexID, uint iid : SV_InstanceID,
                out float4 outPosition : SV_Position)
    {
        float4 data = _LineData[uint2(iid, 0)];
        float4 p = float4(0, 0, 0, 1);
        p.xy = (vid == 0 ? data.xy : data.zw);
        outPosition = UnityObjectToClipPos(p);
    }

    float4 Fragment(float4 position : SV_Position) : SV_Target
    {
        return _LineColor;
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
