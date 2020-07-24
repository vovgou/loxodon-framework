Shader "Mobile/Terrain" {
Properties {
	_Splat0 ("Layer 1", 2D) = "white" {}
	_Splat1 ("Layer 2", 2D) = "white" {}
	_Splat2 ("Layer 3", 2D) = "white" {}
	_Control ("Control (RGBA)", 2D) = "white" {}
}

SubShader {
	Tags {
		"RenderType" = "Opaque"
	}
	LOD 150

	CGPROGRAM 
	#pragma surface surf Lambert
	#pragma exclude_renderers xbox360 ps3 flash d3d11_9x 

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0: TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
};

uniform sampler2D _Control;
uniform sampler2D _Splat0, _Splat1,_Splat2;

void surf (Input IN, inout SurfaceOutput o) {

	fixed4 splat_control = tex2D (_Control, IN.uv_Control);
		
//	fixed4 lay1 = tex2D (_Splat0, IN.uv_Splat0);
//	fixed4 lay2 = tex2D (_Splat1, IN.uv_Splat1);
//	fixed4 lay3 = tex2D (_Splat2, IN.uv_Splat2);
	
	fixed4 c = tex2D (_Splat0, IN.uv_Splat0) * splat_control.r;
	c += tex2D (_Splat1, IN.uv_Splat1) * splat_control.g;
	c += tex2D (_Splat2, IN.uv_Splat2) * splat_control.b;
		
	o.Albedo = c.rgb;
//	o.Alpha = 1.0;
//	o.Albedo = (lay1 * splat_control.r + lay2 * splat_control.g+ lay3 * splat_control.b);	
}
	
ENDCG  
}
Fallback "Mobile/Diffuse"
}
