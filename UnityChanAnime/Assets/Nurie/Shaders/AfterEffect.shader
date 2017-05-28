Shader "Custom/AfterEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurTex ("blur tex", 2D) = "black" {}
		
		
		_P1 ("p1", Float) = 0
		_P2 ("p2", Float) = 0
		_P3 ("p3", Float) = 0
		_P4 ("p4", Float) = 0
		
		_P5 ("p5", Float) = 0
		_P6 ("p6", Float) = 0
		_P7 ("p7", Float) = 0
		_P8 ("p8", Float) = 0
		
		_P9 ("p9", Float) = 0
		_P10 ("p10", Float) = 0
	}
	CGINCLUDE
		#include "UnityCG.cginc"
 		float
 			_P1,_P2,_P3,_P4,
 			_P5,_P6,_P7,_P8,
 			_P9,_P10;
		sampler2D _MainTex,_BlurTex;
		half4 _MainTex_TexelSize;
			
		fixed4 frag(v2f_img i) : COLOR{
			float2 uv = i.uv;
			float2 uv2 = uv;
			#if UNITY_UV_STARTS_AT_TOP
			if(_MainTex_TexelSize.y<0.0)
				uv2.y = 1.0-uv2.y;
			#endif
			fixed4 c = tex2D(_MainTex, uv);
			fixed4 b = tex2D(_BlurTex, uv2);
			float d = distance(float2(0.5,0.5),uv);
			
			c = lerp(c,b,saturate(d*d));
			c = c*saturate(b*4) + b*d*d;
			
			float4 f = c*4.0;
			int4 fi = floor(f);
			b = (fi+pow(smoothstep(fi,fi+1,f),2))/3.0;
			c = lerp(c,b,0.5*d*d);
			
			
				float
					x = uv2.x,
					y = uv2.y;
				if(x < 1.0/3.0){
					if(y < lerp(_P1,_P2,x*3))
						c *= saturate(1.0-(lerp(_P1,_P2,x*3.0)-y)/abs(_MainTex_TexelSize.y));
					else if(y > lerp(1.0-_P5,1.0-_P6,x*3.0))
						c *= saturate(1.0-(y-lerp(1.0-_P5,1.0-_P6,x*3.0))/abs(_MainTex_TexelSize.y));
				}
				else if(x < 2.0/3.0){
					if(y < lerp(_P2,_P3,(x-1.0/3.0)*3.0))
						c *= saturate(1.0-(lerp(_P2,_P3,(x-1.0/3.0)*3.0)-y)/abs(_MainTex_TexelSize.y));
					else if(y > lerp(1.0-_P6,1.0-_P7,(x-1.0/3.0)*3.0))
						c *= saturate(1.0-(y-lerp(1.0-_P6,1.0-_P7,(x-1.0/3.0)*3.0))/abs(_MainTex_TexelSize.y));
				}
				else{
					if(y < lerp(_P3,_P4,(x-2.0/3.0)*3.0))
						c *= saturate(1.0-(lerp(_P3,_P4,(x-2.0/3.0)*3.0)-y)/abs(_MainTex_TexelSize.y));
					else if(y > lerp(1.0-_P7,1.0-_P8,(x-2.0/3.0)*3.0))
						c *= saturate(1.0-(y-lerp(1.0-_P7,1.0-_P8,(x-2.0/3.0)*3.0))/abs(_MainTex_TexelSize.y));
				}
				
				if(_P9 > x)
					c *= 0;
				if(1-_P10 < x)
					c *= 0;
				
				return c;
		}
	ENDCG
	
	SubShader {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }  
		ColorMask RGB
 
		pass{
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			ENDCG
		}
	} 
}