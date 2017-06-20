Shader "SpaceRaceAlpha/Gravity"
{
	Properties
	{
		_Mass ("Mass", Float) = 1
		_GravitationalConstant ("GravitationalConstant", Float) = .1
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex("Main Texture", 2D) = "White" {}
		_Scale ("Scale", Range(0,1)) = .5
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float3 normal: NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 col: COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 _Color;
			sampler2D _MainTex;
			float _Mass;
			float _GravitationalConstant;
			float _Scale;

			uniform float4 _LightColor0;

			fixed4 frag (v2f i) : SV_Target
			{
				float distance = sqrt(pow((i.uv.x -.5),2) * pow((i.uv.y -.5),2));
				float4 col = float4(((_Mass * _GravitationalConstant * _Scale)/pow(distance,2)),0,0,((_Mass * _GravitationalConstant * _Scale)/pow(distance,2)));
				//fixed4 col = float4(sqrt(pow((i.vertex.x -.5),2)) * _Scale,sqrt(pow((i.vertex.y -.5),2)) * _Scale,0,1);
				return col;
			}
			ENDCG
		}
	}
}
