Shader "Custom/UnlitUVOffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _UVOffset ("UV Offset", Vector) = (0, 0, 0, 0)
        _Color ("Tint", Color) = (1,1,1,1)
        _BlackCutoff ("Black Threshold", Range(0,0.1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _UVOffset;
            float4 _Color;
            float _BlackCutoff;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);

                // Standard tiling + offset + custom UV offset
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw + _UVOffset.xy;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * _Color;

                // Convert near-black to transparent
                float brightness = dot(col.rgb, float3(0.299, 0.587, 0.114));
                if (brightness < _BlackCutoff)
                {
                    col.a = 0;
                }

                return col;
            }

            ENDHLSL
        }
    }
}
