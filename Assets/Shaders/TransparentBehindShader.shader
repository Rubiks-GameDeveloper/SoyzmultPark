Shader "Custom/TransparentBehindShaderURP" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {} // Текстура объекта
        _BehindAlpha ("Transparency Behind", Range(0,1)) = 0.0 // Прозрачность позади occluder
    }
    SubShader {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            ZWrite Off // Не писать в depth
            Blend SrcAlpha OneMinusSrcAlpha // Альфа-блендинг
            Stencil {
                Ref 1 // То же значение, что у occluder
                Comp Equal // Прозрачность, если stencil = 1
                Pass Keep // Не менять stencil
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _BehindAlpha;

            Varyings vert (Attributes IN) {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                // Если в области occluder (stencil = 1), применяем прозрачность
                col.a = lerp(col.a, _BehindAlpha, 1.0); // Плавный переход
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}