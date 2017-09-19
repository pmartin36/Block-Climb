// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Player/BlackBehindSmoke"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
			"DisableBatching" = "True"
		}
		Pass
		{
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 screenuv : TEXCOORD1;
				half4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) * 0.5;
				o.color = v.color;
				return o;
			}
			
			sampler2D _MainTex;
			uniform sampler2D _GlobalRefractionTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_GlobalRefractionTex, i.screenuv);
				float a = 1-col.a;

				//if(col.a > 0.9)
					return fixed4(0.5,0.5,0.5,col.a*5);
					//return fixed4(0,0,0,col.a);
				//else
					//return fixed4(0,0,0,0);
			}
			ENDCG
		}
	}
}
