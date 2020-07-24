// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "mask_blend" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Main Texture", 2D) = "white" {}
 _MaskTex ("Mask Texture (RG)", 2D) = "white" {}
 _ScrollTimeX  ("Scroll X Factor", Float) = 0
 _ScrollTimeY  ("Scroll Y Factor", Float) = 0
}

Category {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Blend SrcAlpha One
 Cull Off Lighting Off ZWrite Off
 BindChannels {
     Bind "Color", color
     Bind "Vertex", vertex
     Bind "TexCoord", texcoord
 }
 
 // ---- Fragment program cards
 SubShader {
     Pass {
     
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #pragma fragmentoption ARB_precision_hint_fastest
         #pragma multi_compile_particles
         
         #include "UnityCG.cginc"

         sampler2D _MainTex;
         sampler2D _MaskTex;
         half _ScrollTimeX;
         half _ScrollTimeY;
         fixed4 _TintColor;
         
         struct appdata_t {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
             float2 texcoord : TEXCOORD0;
         };

         struct v2f {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
             float2 texcoord : TEXCOORD0;
         };
         
         v2f vert (appdata_t v)
         {
             v2f o;
             o.vertex = UnityObjectToClipPos(v.vertex);
             o.color = v.color;
             o.texcoord = v.texcoord;
             return o;
         }
         fixed4 frag (v2f i) : COLOR
         {
             half2 uvoft = i.texcoord;
             uvoft.x += _Time.yx*_ScrollTimeX;
             uvoft.y += _Time.yx*_ScrollTimeY;
             fixed4 offsetColor = tex2D(_MaskTex, uvoft);
             fixed grayscale = Luminance(offsetColor.rgb);
             fixed4 mainColor = tex2D(_MainTex, i.texcoord);
             return 2.0f * i.color * _TintColor * mainColor * grayscale;
         }
         ENDCG
     }
 }   
}
}