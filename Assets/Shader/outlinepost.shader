Shader "Hidden/OutlinePost"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _Thickness("Thickness", Float) = 1.0
    }

        SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType" = "Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "OutlinePost"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalsTexture;
            float4 _OutlineColor;
            float _Thickness;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 texel = _Thickness / _ScreenParams.xy;

                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                float3 normal = tex2D(_CameraNormalsTexture, uv).xyz;

                float edge = 0;

                // 簡單比較周圍像素深度和法線
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = uv + float2(x, y) * texel;
                        float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, offset);
                        float3 n = tex2D(_CameraNormalsTexture, offset).xyz;

                        if (abs(d - depth) > 0.01 || distance(n, normal) > 0.1)
                        {
                            edge = 1;
                        }
                    }
                }

                if (edge > 0)
                    return _OutlineColor;
                else
                    return float4(0,0,0,0);
            }
            ENDHLSL
        }
    }
}
