Shader "displacement additive" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Main Texture", 2D) = "white" {}
 _DispMap ("Displacement Map (RG)", 2D) = "white" {}
 _MaskTex ("Mask(R)", 2D) = "white" {}
 _DispScrollSpeedX  ("Map Scroll Speed X", Float) = 0
 _DispScrollSpeedY  ("Map Scroll Speed Y", Float) = 0
 _DispX  ("Displacement Strength X", Float) = 0
 _DispY  ("Displacement Strength Y", Float) = 0.2

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

         uniform sampler2D _MainTex;
		 uniform sampler2D _DispMap;
		 uniform sampler2D _MaskTex;
         uniform half _DispScrollSpeedX;
         uniform half _DispScrollSpeedY;

		 uniform half _DispX;
         uniform half _DispY;


         uniform fixed4 _TintColor;

         
         struct appdata_t {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
			 float2 texcoord : TEXCOORD0;
             float2 param : TEXCOORD1;
         };

         struct v2f {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
             float2 texcoord : TEXCOORD0;
			 float2 param : TEXCOORD1;
         }; 
         
         v2f vert (appdata_t v)
         {
             v2f o;
             o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
             o.color = v.color;
             o.texcoord = v.texcoord;
			 o.param = v.param;
             return o;
         }
         fixed4 frag (v2f i) : COLOR
         {
		     //scroll displacement map.
			 half2 mapoft = half2(_Time.y*_DispScrollSpeedX, _Time.y*_DispScrollSpeedY);

			 //get displacement color
			 half4 dispColor = tex2D(_DispMap, i.texcoord + mapoft);

			 //get uv oft
             half2 uvoft = i.texcoord;

		     uvoft.x +=  dispColor.r  * _DispX * i.param.x;
			 uvoft.y +=  dispColor.g  * _DispY * i.param.x;


			 //apply displacement
			 fixed4 mainColor = tex2D(_MainTex, uvoft);

			 //get mask;
			 fixed4 mask = tex2D(_MaskTex, i.texcoord);

             return 2.0f * i.color * _TintColor * mainColor * mask.r;
         }
         ENDCG
     }
 }   
}
}