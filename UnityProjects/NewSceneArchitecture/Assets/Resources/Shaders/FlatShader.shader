/*
   Created by Vilas Tewari on 2009-08-21.

   	A shader that just outputs texture.
	Adapted from Unity's ShaderLab reference
*/

Shader "Flat Shader"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white"{}
	}
	
	Category
	{
		Blend AppSrcAdd AppDstAdd
	    Fog { Color [_AddFog] }
	
		SubShader
		{
			// Pixel Lights Pass
			Pass {
				Name "Base"
				Tags { "LightMode" = "Pixel" }
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_builtin
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				#include "AutoLight.cginc" 

				struct v2f
				{
					V2F_POS_FOG;
					LIGHTING_COORDS
					float2 uv;
				}; 

				uniform float4 _MainTex_ST;
				uniform float4 _Color;

				v2f vert (appdata_base v)
				{
					v2f o;
					PositionFog( v.vertex, o.pos, o.fog );
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				uniform sampler2D _MainTex;

				float4 frag (v2f i) : COLOR
				{
					return tex2D( _MainTex, i.uv ) * LIGHT_ATTENUATION(i) * _Color;
				}
				ENDCG
			}
		
			// Pass to render object as a shadow caster
	        Pass {
	           Name "ShadowCaster"
	           Tags { "LightMode" = "ShadowCaster" }

	           Fog {Mode Off}
	           ZWrite On ZTest Less Cull Off
	           Offset [_ShadowBias], [_ShadowBiasSlope]

	           CGPROGRAM
	           #pragma vertex vert
	           #pragma fragment frag
	           #pragma multi_compile SHADOWS_NATIVE SHADOWS_CUBE
	           #pragma fragmentoption ARB_precision_hint_fastest
	           #include "UnityCG.cginc"

	           struct v2f {
	               V2F_SHADOW_CASTER;
	           };

	           v2f vert( appdata_base v )
	           {
	               v2f o;
	               TRANSFER_SHADOW_CASTER(o)
	               return o;
	           }

	           float4 frag( v2f i ) : COLOR
	           {
	               SHADOW_CASTER_FRAGMENT(i)
	           }
	           ENDCG
	       }

	       // Pass to render object as a shadow collector
	       Pass {
	            Name "ShadowCollector"
	            Tags { "LightMode" = "ShadowCollector" }

	            Fog {Mode Off}
	            ZWrite On ZTest Less

	            CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma fragmentoption ARB_precision_hint_fastest

	            #define SHADOW_COLLECTOR_PASS
	            #include "UnityCG.cginc"

	            struct appdata {
	                float4 vertex;
	            };

	            struct v2f {
	                V2F_SHADOW_COLLECTOR;
	            };

	            v2f vert (appdata v)
	            {
	                v2f o;
	                TRANSFER_SHADOW_COLLECTOR(o)
	                return o;
	            }

	            half4 frag (v2f i) : COLOR
	            {
	                SHADOW_COLLECTOR_FRAGMENT(i)
	            }
	            ENDCG
	        }
		}
		
		// Dinosaur
		SubShader
		{
			Pass
			{
				Name "Base"
				
				// Apply base texture
	            SetTexture [_MainTex]
				{
	                combine texture
	            }
			}
		}
	}
}
