Shader "Custom/UnlitTransparent"
{
  Properties
  {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" {}
    _GlowRadius ("Glow Scale", float) = 0
    
    [Enum(Off,0,On,1)] 
    _ZWrite ("ZWrite", Float) = 1
    
    [Enum(Always, 0, Less, 2, Equal, 3, LEqual, 4, GEqual, 5)] 
    _ZTest ("ZTest", Float) = 4
  }
  SubShader
  {
    Tags { "RenderType"="Transparent" "Queue"="Transparent" }
    Blend SrcAlpha OneMinusSrcAlpha
    ZWrite [_ZWrite]
    ZTest [_ZTest]

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"
      #include "CellShading.cginc"

      struct appdata
      {
        float4 vertex : POSITION;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
      };

      struct v2f
      {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        fixed4 color : COLOR;
        float4 worldPos : TEXCOORD1;
      };

      sampler2D _MainTex;
      float4 _MainTex_ST;
      float4 _Color;
      float4 _TintColor;
      float _GlowRadius;

      v2f vert (appdata v)
      {
        v2f o;
        o.worldPos = mul(unity_ObjectToWorld, v.vertex);
        o.pos = UnityWorldToClipPos(o.worldPos);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.color = v.color;

        return o;
      }

      fixed4 frag (v2f i) : SV_Target
      {
        float4 color = _Color * tex2D(_MainTex, i.uv) * i.color;
        return fixed4(color.rgb * _GlowRadius, color.a);
      }
      ENDCG
    }
  }
}
