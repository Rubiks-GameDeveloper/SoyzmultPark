Shader "Custom/OccluderShader" {
    Properties {
        // Нет свойств, так как invisible
    }
    SubShader {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 100

        // Писать в depth buffer
        ZWrite On
        // Не писать в color buffer
        ColorMask 0

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return fixed4(0,0,0,0); // Невидимый
            }
            ENDCG
        }
    }
}