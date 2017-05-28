Shader "Unlit/UnlitShaderMoveUV"
{
	Properties
	{
	    _TintColor("Tint Color",Color) = (1.,1.,1.,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainOfsX("Main Offset X Dir", Range(-1.0,1.0)) = 1.0
		_MainOfsY("Main Offset Y Dir", Range(-1.0,1.0)) = 1.0
		_MainOfsSpd("Main Offset Speed", Float) = 1.0
		_AlphaTex ("Alpha Texture", 2D) = "white" {}
		_AlphaOfsX("Alpha Offset X Dir", Range(-1.0,1.0)) = 1.0
		_AlphaOfsY("Alpha Offset Y Dir", Range(-1.0,1.0)) = 1.0
		_AlphaOfsSpd("Alpha Offset Speed", Float) = 1.0
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	    LOD 100

		ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float4 _TintColor;
			sampler2D _MainTex;	
			sampler2D _AlphaTex;
			float4 _MainTex_ST;
			float4 _AlphaTex_ST;
			float _MainOfsX;
			float _MainOfsY;
			float _MainOfsSpd;
			float _AlphaOfsX;
			float _AlphaOfsY;
			float _AlphaOfsSpd;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv += float2(_Time.x*_MainOfsX,_Time.x*_MainOfsY) * _MainOfsSpd;
				o.uv1 = TRANSFORM_TEX(v.uv, _AlphaTex);
				o.uv1 += float2(_Time.x*_AlphaOfsX,_Time.x*_AlphaOfsY) * _AlphaOfsSpd;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col1 = tex2D(_AlphaTex, i.uv1);
				//col = lerp(col,col1,col1.a);
				col.a = col1.a;
				col *= _TintColor;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
