Shader "Custom/GBTWater" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Distortion ("Distortion", Range(0, 1)) = 0.1
        _Speed ("Speed", Range(0, 10)) = 1
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Distortion;
            float _Speed;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;
                uv.x += _Time.y * _Speed;
                uv.y += sin(uv.x + _Time.y * _Speed) * _Distortion;
                float4 col = _Color * tex2D(_MainTex, uv).r;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
