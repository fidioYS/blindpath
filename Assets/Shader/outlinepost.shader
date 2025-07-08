diff --git a / Assets / Shader / outlinepost.shader b / Assets / Shader / outlinepost.shader
index 7288e141ac1e4467c0febf2c050bd2ef4c2b1508..f87aecada3d3ecdb70c501890365afa2027a5cf4 100644
-- - a / Assets / Shader / outlinepost.shader
++ + b / Assets / Shader / outlinepost.shader
@@ - 9, 73 + 9, 73 @@ Shader "Hidden/OutlinePost"
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

-sampler2D _CameraDepthTexture;
-sampler2D _CameraNormalsTexture;
+TEXTURE2D(_CameraDepthTexture);
+SAMPLER(sampler_CameraDepthTexture);
+TEXTURE2D(_CameraNormalsTexture);
+SAMPLER(sampler_CameraNormalsTexture);
             float4 _OutlineColor;
             float _Thickness;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D(_CameraNormalsTexture);
            SAMPLER(sampler_CameraNormalsTexture);
            float4 _OutlineColor;
            float _Thickness;
=======
=======
>>>>>>> Stashed changes
             v2f vert(appdata v)
             {
                 v2f o;
                 o.pos = TransformObjectToHClip(v.vertex.xyz);
                 o.uv = v.uv;
                 return o;
             }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

             half4 frag(v2f i) : SV_Target
             {
                 float2 uv = i.uv;
                 float2 texel = _Thickness / _ScreenParams.xy;

                 float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
-float3 normal = tex2D(_CameraNormalsTexture, uv).xyz;
+float3 normal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv).xyz;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                float3 normal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv).xyz;

                float edge = 0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = uv + float2(x, y) * texel;
                        float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, offset);
                        float3 n = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, offset).xyz;

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
=======
                 float edge = 0;
-
-float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, offset);
                 for (int x = -1; x <= 1; x++)
                 {
                     for (int y = -1; y <= 1; y++)
                     {
                         float2 offset = uv + float2(x, y) * texel;
-float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, offset);
-float3 n = tex2D(_CameraNormalsTexture, offset).xyz;
+float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, offset);
+float3 n = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, offset).xyz;

=======
                 float edge = 0;
-
-float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, offset);
                 for (int x = -1; x <= 1; x++)
                 {
                     for (int y = -1; y <= 1; y++)
                     {
                         float2 offset = uv + float2(x, y) * texel;
-float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, offset);
-float3 n = tex2D(_CameraNormalsTexture, offset).xyz;
+float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, offset);
+float3 n = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, offset).xyz;

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
}
 }
