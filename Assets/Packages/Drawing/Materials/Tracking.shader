Shader "Custom/Tracking" {
	Properties {
		_MainTex ("Main Tex", 2D) = "black" {}
		_EquilColor ("Equil Color", Color) = (0, 0, 0, 0.01)
		_Additive ("Additive", Float) = 0.1
        _TrailDim ("Trail Dim", Range(0, 1)) = 1
	}
	SubShader {
		ZTest Always ZWrite Off Cull Off Fog { Mode Off }
		
		CGINCLUDE
		struct vsin {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};
		struct vs2ps {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};
		
		sampler2D _MainTex;
		float4 _EquilColor;
		float _Additive;
        float _TrailDim;

		vs2ps vert(vsin IN) {
			vs2ps OUT;
			OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
			OUT.uv = IN.uv;
			return OUT;
		}
		ENDCG
		
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 frag(vs2ps IN) : COLOR {
				return float4(_EquilColor.rgb, _EquilColor.a * _TrailDim);
			}
			ENDCG
		}
		
		Pass {
			Blend SrcAlpha One
					
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 frag(vs2ps IN) : COLOR {
				float4 c = tex2D(_MainTex, IN.uv);
				return float4(c.rgb, _Additive);
			}
			ENDCG
		}
	} 
	FallBack Off
}
