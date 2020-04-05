Shader "Avatar/Fashion Model"
{
	Properties
	{
		_Falloff ("Saturation Falloff", Range (0.5, 3)) = 1
		_Cuttoff ("Saturation Cutoff", Range (0, 1)) = 1
		_Selection ("Selection Effect Amount", Range(0, 1)) = 0
		_SelectionColor ("Selection Color", Color) = (0.15,1,.25,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			Name "BASE"

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Falloff;
			uniform float _Cuttoff;
			uniform float _Selection; // Range [0 .. 1]
			uniform float4 _SelectionColor;

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

				float ndl = dot( i.normal, i.fakeLight );
				half4 rim = clamp( ndl, 0, 1 ) / 2.2;
				result = lerp(result, _SelectionColor, _Selection);
				
				
				// Add Rim light
				result += rim + (rim * _Selection);
				
				return result;
			}

			// End CG compilation
			ENDCG
		}   
	}

	// Fallback
	Fallback "Flat Shader"
}