Shader "Custom/WallWithDecalsColor"
{
    Properties
    {
        _WallColor ("Wall Color", Color) = (0.7,0.7,0.7,1)
        _MaskRT ("Decals RenderTexture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        fixed4 _WallColor;
        sampler2D _MaskRT;

        struct Input
        {
            float2 uv_MaskRT;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 decalCol = tex2D(_MaskRT, IN.uv_MaskRT);

            fixed4 baseCol = _WallColor;
            fixed4 finalCol = baseCol;

            // накладываем чёрные отпечатки
            finalCol.rgb = lerp(baseCol.rgb, decalCol.rgb, decalCol.a);

            o.Albedo = finalCol.rgb;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}