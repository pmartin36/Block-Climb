// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/CollectableBlock"
{
	Properties
	{
		_MainTex ("Main Tex", 2D) = "white" {}
		_Block ("Inner Block", 2D) = "white" {}

		_Color("Color", Color) = (1,1,1,1)
		_Border("Border", 2D) = "white" {}
		_BorderColor("Border Color", Color) = (1,1,1,1)

		_DistanceBasedOpacity("Distance Alpha", float) = 0
		_Distance("Distance", Range(-10.0,10.0)) = 0

		_Angle ("Angle", Range(-5.0, 5.0)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
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

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 alpha_uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _Block;

			sampler2D _Border;
			float4 _BorderColor;
			float4 _Color;

			float _Distance;
			float _DistanceBasedOpacity;

			float _Angle;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = v.texcoord.xy;

				//pivot point
				float2 pivot = float2(0.5, 0.5);

				// Rotation Matrix
				float cosAngle = cos(_Angle);
				float sinAngle = sin(_Angle);
				float2x2 rot = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);

				// Rotation consedering pivot
				float2 uv = v.texcoord.xy - pivot;
				o.alpha_uv = mul(rot, uv);
				o.alpha_uv += pivot;

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				float4 border = tex2D(_Border, i.uv) * _BorderColor;
				border *= border.a;
				float4 col = tex2D(_Block, i.uv) * (1 - border.a);

				col = (col + border) * _Color;

				if (_DistanceBasedOpacity > 0) {
					col.a *= 1 - abs((_Distance + 4 * i.alpha_uv.x) / 10.0);
				}

				return col;
			}
			ENDCG
		}
	}
}
