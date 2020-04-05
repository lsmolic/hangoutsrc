/*
	Created by Vilas Tewari on 2009-05-01.

	This Shader calcualtes a lambertian coefficient
	and uses it to darken and saturate shader output
	as the fragment normal faces away from the camera
	
	It also has a built in rim light.
	
	The shader falls back to Vertex Lit
*/
	
Shader "Avatar/Saturation Shader"
{
	// like public vars of c# scripts
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_Falloff ("Saturation Falloff", Range (0.5, 3)) = 1
		_Cuttoff ("Saturation Cutoff", Range (0, 1)) = 1
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			Name "BASE"

			// Tag to compile in CG compiler
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// uniform variable filled in by property
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Falloff;
			uniform float _Cuttoff;
			uniform float4 _Color;

			// Output struct
			struct v2f
			{
				V2F_POS_FOG;
				float3 viewDir;
				float2 uv;
				float3 normal;
				float3 fakeLight;
			};

			v2f vert ( appdata_base v )
			{
				// Set position & Fog
				v2f o;
				PositionFog( v.vertex, o.pos, o.fog );

				o.normal = v.normal;
				o.viewDir = -ObjSpaceViewDir( v.vertex );

				// incident ray from fake rim light
				o.fakeLight = normalize( o.viewDir - cross( o.viewDir, half3( 0, 1, 0 ) ) );
				o.uv = TRANSFORM_TEX( v.texcoord, _MainTex );
				return o;
			}

			half4 frag (v2f i) : COLOR
			{   
				// Get Texture color
				half4 result = tex2D( _MainTex, i.uv );

				/*
					Use the inverse of lambertian Coefficient to saturate the result
					Calculate lambertain coefficient using Camera as light source.
					Could have used a Frenel Coefficient here but
					lambertain ends up looking better on low poly
				*/
				float lambertianCoef = ( dot( i.normal, normalize(i.viewDir) ) + 1 )/ 2;
				lambertianCoef = pow( lambertianCoef, _Falloff );

				/*
					Find Max of R, G, B
				*/
				float maxValue = max( result.r, result.g );
				maxValue = max( maxValue, result.b );

				/*
					Saturate result based on the lambertian coefficeint
					Normally you saturate RGB using the middle Color value as base luminance
					However the result of using max looks better since it saturates and darkens the color
				*/
				half4 saturated = result;
				saturated = ( result * 2 ) - maxValue;
				saturated = ( saturated * 2 ) - maxValue;
				result = lerp( result, saturated, clamp( lambertianCoef, 0, _Cuttoff ) );

				// Add Rim light
				result += clamp( dot( i.normal, i.fakeLight ), 0, 1 ) / 1.5;
				return result * _Color;
			}

			// End CG compilation
			ENDCG
		}   
	}

	// Fallback
	Fallback "Flat Shader"
}