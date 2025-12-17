Shader "Custom/UnlitGridUVOffset_AA"
{
    Properties
    {
        _Color      ("Line Color", Color) = (1,1,1,1)
        _Background ("Background Color", Color) = (0,0,0,1)
        _GridSize   ("Grid Size", Float) = 1
        _LineWidth  ("Line Width", Float) = 0.02
        _UVOffset   ("UV Offset", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Background"
            "RenderType"="Opaque"
        }

        ZWrite Off
        Blend Off

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

            float4 _Color;
            float4 _Background;
            float  _GridSize;
            float  _LineWidth;
            float4 _UVOffset;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv + _UVOffset.xy;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // Scale UVs into grid space
                float2 gridUV = i.uv / _GridSize;

                // Distance to nearest grid line
                float2 g = abs(frac(gridUV) - 0.5);
                float lineDist = max(g.x, g.y);

                // Screen-space derivatives for AA
                float fw = max(fwidth(gridUV.x), fwidth(gridUV.y));

                // Anti-aliased line mask
                float lineMask = smoothstep(
                    0.5 - _LineWidth - fw,
                    0.5 - _LineWidth + fw,
                    lineDist
                );

                // Grid lines over background
                return lerp(_Color, _Background, lineMask);
            }

            ENDHLSL
        }
    }
}
