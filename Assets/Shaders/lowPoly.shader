Shader "Custom/lowPoly"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB)", 2D) = "white" { }
    }

        CGINCLUDE
#include "UnityCG.cginc"
        ENDCG

        SubShader
    {
        Tags {"Queue" = "Overlay" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            ENDCG

            SetTexture[_MainTex] { combine primary }
        }
    }

        SubShader
    {
        Tags {"Queue" = "Overlay" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void vert(inout appdata_full v)
        {
            v.normal = float3(0,1,0);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Colorear el objeto con el color especificado
            o.Albedo = _Color.rgb;

            // Sombras y luces simples
            o.Normal = float3(0, 1, 0);
            o.Alpha = _Color.a;
        }
        ENDCG
    }
}
