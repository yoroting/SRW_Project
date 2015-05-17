Shader "Custom/GrayLevel (AlphaClip)" {
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
					
					float2 factor = abs(i.worldPos);
					float val = 1.0 - max(factor.x, factor.y);

					// Option 1: 'if' statement
					if (val < 0.0) col.a = 0.0;
	
					// Option 2: no 'if' statement -- may be faster on some devices
					//col.a *= ceil(clamp(val, 0.0, 1.0));
				
				
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
