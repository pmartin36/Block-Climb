// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/OutlineReplace"
{
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Outlinable" = "True"
			"Queue" = "Transparent"
		}

		Cull Off
		ZWrite Off
		ZTest Always
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 _GlowColor;

			fixed4 frag (v2f i) : SV_Target
			{
				float r = ((sin(_Time.w) + 3) * 0.1) + 0.2;
				r = r * _GlowColor;
				return fixed4(r,r,r,1);
			}
			ENDCG
		}
	}
}
