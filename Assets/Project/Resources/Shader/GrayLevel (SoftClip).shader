Shader "Custom/GrayLevel (SoftClip)" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_GrayLevelScale ("Gray level scale", Range (0.0, 1.0)) = 0.0
	}
	
	SubShader
	{
		LOD 100
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 worldPos : TEXCOORD1;
				};
	
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _GrayLevelScale;
				
				float2 _ClipSharpness = float2(20.0, 20.0);
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;
					o.texcoord = v.texcoord;
					o.worldPos = TRANSFORM_TEX(v.vertex.xy, _MainTex);
				
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					// Sample the texture
					fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
					
					// Softness factor
					float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipSharpness;
					col.a *= clamp( min(factor.x, factor.y), 0.0, 1.0);
					
					// Caculate gray level color
					fixed grayLevel = 0.299*col.r + 0.587*col.g + 0.114*col.b;
					fixed4 grayCol = fixed4(grayLevel, grayLevel, grayLevel, col.a);
					
					return lerp(col, grayCol, _GrayLevelScale);
				}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
