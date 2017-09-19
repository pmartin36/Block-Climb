// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/OutlineComposite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off 
		ZWrite On 
		ZTest Always

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
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			float2 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = v.uv;
				o.uv1 = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - o.uv1.y;
				#endif

				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _OutlinePrePassTex;
			sampler2D _OutlineBlurredTex;
			float _Intensity;
			float _PercentShine;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv0);

				fixed4 outline = tex2D(_OutlineBlurredTex, i.uv1);
				fixed4 prepass = tex2D(_OutlinePrePassTex, i.uv1);

				//fixed4 glow = max(0, outline - prepass);
				fixed4 glow = max(0.05*prepass.r, outline - prepass);

				return (col + glow * _Intensity);
			}
			ENDCG
		}
	}
}
