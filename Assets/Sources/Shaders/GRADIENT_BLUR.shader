Shader "Custom/GradientBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color1", Color) = (0.525, 0.443, 0.925, 0.2)
        _Color2 ("Color2", Color) = (0.427, 0.275, 0.612, 1)
        _BumpAmt  ("Distortion", Range (0,128)) = 10
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _Size ("Size", Range(0, 20)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color1;
            fixed4 _Color2;
            float _BumpAmt;
            sampler2D _BumpMap;
            float _Size;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 _MainTex_TexelSize;
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = lerp(_Color1, _Color2, i.uv.y);
                half2 bump = UnpackNormal(tex2D( _BumpMap, i.uv )).rg;
                float2 offset = bump * _BumpAmt * _MainTex_TexelSize.xy;
                i.uv.xy = offset * i.uv.z + i.uv.xy;
                fixed4 blur = tex2Dproj( _MainTex, float4(i.uv.x, i.uv.y, i.uv.z, 1.0));
                return tex * col * blur;
            }
            ENDCG
        }
    }
}