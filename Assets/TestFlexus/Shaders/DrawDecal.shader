Shader "Custom/DrawDecal"
{
    Properties
    {
        _DecalTex ("Decal Texture", 2D) = "white" {}
        _UV ("UV Position", Vector) = (0,0,0,0)
        _Size ("Decal Size", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _DecalTex;
            float4 _UV;
            float _Size;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                o.pos = float4((id == 1 || id == 2) ? 3 : -1,
                               (id >= 2) ? 3 : -1, 0, 1);
                o.uv = float2((id == 1 || id == 2), (id >= 2));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // смещение текстуры так, чтобы рисовать около _UV
                float2 uv = (i.uv - _UV.xy) / _Size + 0.5;

                fixed4 col = tex2D(_DecalTex, uv);
                return col; // просто добавляем текстуру (например чёрный кружок с альфой)
            }
            ENDCG
        }
    }
}