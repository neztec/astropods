Shader "Custom/UnlitGridUVOffset"
{
    Properties
    {
        _Color ("Line Color", Color) = (1,1,1,1)
        _Background ("Background Color", Color) = (0,0,0,1)
        _GridSize ("Grid Size", Float) = 1
        _LineWidth ("Line Width", Float) = 0.02
        _UVOffset ("UV Offset", Vector) = (0,0,0,0)
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

            float4 _Color;
            float4 _Background;
            float _GridSize;
            float _LineWidth;
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
                float2 gridUV = i.uv / _GridSize;

                float2 g = abs(frac(gridUV) - 0.5);
                float lineMask = step(0.5 - _LineWidth, max(g.x, g.y));

                return lerp(_Color, _Background, lineMask);
            }

            ENDHLSL
        }
    }
}
