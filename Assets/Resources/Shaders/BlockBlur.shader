// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BlockBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		// Horizontal
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}					

			sampler2D _MainTex;
			float2 _BlurSize;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 s = tex2D(_MainTex, i.uv) * 0.38774;

				fixed4 blur = tex2D(_MainTex, i.uv + float2(_BlurSize.x * 2, 0)) * 0.06136;
				blur += tex2D(_MainTex, i.uv + float2(_BlurSize.x, 0)) * 0.24477;
				blur += tex2D(_MainTex, i.uv + float2(_BlurSize.x * -1, 0)) * 0.24477;
				blur += tex2D(_MainTex, i.uv + float2(_BlurSize.x * -2, 0)) * 0.06136;

				//float bmult = (sin(_Time.z) + 1) * 0.1; //0->0.2
				//blur *= bmult + 0.8; //0.8->1

				s += blur;

				return s;
			}
			ENDCG
		}

		// Vertical
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}			

			sampler2D _MainTex;
			float2 _BlurSize;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 s = tex2D(_MainTex, i.uv) * 0.38774;

				fixed4 blur = tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * 2)) * 0.06136;
				blur += tex2D(_MainTex, i.uv + float2(0, _BlurSize.y)) * 0.24477;
				blur += tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * -1)) * 0.24477;
				blur += tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * -2)) * 0.06136;
				
				//float bmult = (sin(_Time.z) + 1) * 0.1; //0->0.2
				//blur *= bmult + 0.8; //0.8->1

				s += blur;

				return s;
			}
			ENDCG
		}
	}
}
