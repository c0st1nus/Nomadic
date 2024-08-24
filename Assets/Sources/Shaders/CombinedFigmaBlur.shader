
Shader "Custom/CombinedFigmaBlur" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _GrabTexture ("GrabTexture", 2D) = "black" {}
        _Size ("Blur Size", Range(0, 10)) = 1.0
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
        
        // Grab the current screen to apply blur
        GrabPass { "_GrabTexture" }
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Size;

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            half4 ApplyBlur(v2f i) {
                half4 sum = half4(0, 0, 0, 0);
                float2 uv = i.texcoord;
                float2 offset = _GrabTexture_TexelSize.xy * _Size;
                
                sum += tex2D(_GrabTexture, uv - offset * 4.0) * 0.05;
                sum += tex2D(_GrabTexture, uv - offset * 3.0) * 0.09;
                sum += tex2D(_GrabTexture, uv - offset * 2.0) * 0.12;
                sum += tex2D(_GrabTexture, uv - offset * 1.0) * 0.15;
                sum += tex2D(_GrabTexture, uv) * 0.18;
                sum += tex2D(_GrabTexture, uv + offset * 1.0) * 0.15;
                sum += tex2D(_GrabTexture, uv + offset * 2.0) * 0.12;
                sum += tex2D(_GrabTexture, uv + offset * 3.0) * 0.09;
                sum += tex2D(_GrabTexture, uv + offset * 4.0) * 0.05;
                
                return sum;
            }

            half4 frag(v2f i) : SV_Target {
                // Apply Figma Image Shader logic here
                half4 color = tex2D(_MainTex, i.texcoord);
                
                // Applying Blur effect
                half4 blurredColor = ApplyBlur(i);
                
                // Combine original color with blurred version
                return lerp(color, blurredColor, 0.5);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
