Shader "Custom/TransparentOccluderShaderURP" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {} // Текстура для occluder (опционально)
        _Alpha ("Transparency", Range(0,1)) = 0.5 // Уровень прозрачности occluder
    }
    SubShader {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On // Писать в depth buffer
            Blend SrcAlpha OneMinusSrcAlpha // Альфа-блендинг для прозрачности
            Stencil {
                Ref 1 // Уникальное значение для stencil
                Comp Always // Всегда проходит тест
                Pass Replace // Заменить значение stencil
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
            float _Alpha;

            Varyings vert (Attributes IN) {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                col.a = _Alpha; // Установить прозрачность
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}