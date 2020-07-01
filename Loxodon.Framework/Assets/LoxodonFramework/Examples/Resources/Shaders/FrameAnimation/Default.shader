// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/FrameAnimation/Default"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

		_RowCount("Row", float) = 0
		_ColCount("Column", float) = 0
		_TotalCount("Count",float) = 0
		_Speed("Speed", float) = 30
   
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
 
        _ColorMask ("Color Mask", Float) = 15
 
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
   
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
 
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
       
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };
       
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
 
                OUT.texcoord = IN.texcoord;
           
                #ifdef UNITY_HALF_TEXEL_OFFSET
                OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
                #endif
           
                OUT.color = IN.color * _Color;
                return OUT;
            }
 
            sampler2D _MainTex;
            float4 _MainTex_TexelSize; //i added
            float4 _MainTex_ST; //i added
			float _Speed;
			float _RowCount;
			float _ColCount;
			float _TotalCount;
 
            fixed4 frag(v2f IN) : SV_Target
            {
				float index = floor((_Time.y * _Speed) % _TotalCount);
				float2 unitSize = float2(1 / _ColCount, 1 / _RowCount);
				float offsetU = floor(index % _ColCount); 
				float offsetV = floor((_TotalCount - 1 - index) / _ColCount);
				float2 originUV = float2(offsetU, offsetV) * unitSize; 
				float2 uv = originUV + IN.texcoord * unitSize;

                half4 color = (tex2D(_MainTex, uv) + _TextureSampleAdd) * IN.color;
           
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
           
                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
 
                return color;
            }
        ENDCG
        }
    }
}