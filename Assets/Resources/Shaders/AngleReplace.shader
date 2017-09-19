Shader "Outline/AngleReplace"
{
	Properties{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"Outlinable" = "True"
		}

		Cull Off
		Blend One OneMinusSrcAlpha

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
			
			float _GlowAngle;
			float2 _GlowAngleVector;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float VectorToAngle(float2 v)
			{
				v.x = sign(v.x) * max(0.000000000001, abs(v.x));
				v.y = sign(v.y) * max(0.000000000001, abs(v.y));
				float d = degrees(atan2(v.y, v.x));
				d = fmod(d + 360, 360);
				return d;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uvn = i.uv * 2 - 1;
				float uva = VectorToAngle(uvn) / 360.0;
				float gaa = _GlowAngle / 360.0;

				float v = ceil(gaa - uva); 

				return float4(v, v, v, 1);
			}
			ENDCG
		}
	}
}
