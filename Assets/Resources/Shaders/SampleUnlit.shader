Shader "Unlit/SampleUnlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Angle("Angle", Range(-6.18, 6.18)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _Angle;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				// Pivot
				float2 pivot = float2(0.5, 0.5);

				// Rotation Matrix
				float cosAngle = cos(_Angle);
				float sinAngle = sin(_Angle);
				float2x2 rot = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);

				// Rotation consedering pivot
				float2 uv = v.texcoord.xy - pivot;
				o.uv = mul(rot, uv);
				o.uv += pivot;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				return float4(i.uv, 1, 1);
				//return col;
			}
			ENDCG
		}
	}
}
