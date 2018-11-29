// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/FrameAnimation/Additive" {
Properties {	
	[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
	_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0

	_RowCount("Row", float) = 0
	_ColCount("Column", float) = 0
	_TotalCount("Count",float) = 0
	_Speed("Speed", float) = 30
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	Blend SrcAlpha One
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			float _Speed;
			float _RowCount;
			float _ColCount;
			float _TotalCount;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
			float _InvFade;
			
			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif

				float index = floor((_Time.y * _Speed) % _TotalCount);
				float2 unitSize = float2(1 / _ColCount, 1 / _RowCount);
				float offsetU = floor(index % _ColCount);
				float offsetV = floor((_TotalCount - 1 - index) / _ColCount);
				float2 originUV = float2(offsetU, offsetV) * unitSize;
				float2 uv = originUV + i.texcoord * unitSize;
				
				fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, uv);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				return col;
			}
			ENDCG 
		}
	}	
}
}
