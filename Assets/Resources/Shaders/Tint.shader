// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Tint"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Tint("Tint Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Cull Off
		ZWrite Off
		ZTest Always

		Blend SrcAlpha OneMinusSrcAlpha

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
				float2 uv : TEXCOORD0;
			};

			fixed4 _Tint;
			sampler2D _MainTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * _Tint;
				//return _ColorAdd;
			}
			ENDCG
		}
	}
}
