Shader "Unlit/FireShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Colour ("Colour", Color) = (1.0, 0.5, 0, 1)
		_Colour2 ("Colour2", Color) = (1.0, 0, 0, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				SHADOW_COORDS(1)
				fixed3 diff : COLOR0;
				fixed3 ambient : COLOR1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Colour;
			float4 _Colour2;
			//Color _Colour;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				//v.vertex.y += sin(_Time.y + v.vertex.y);
				//v.vertex.z += sin(_Time.y + v.vertex.z);

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);//UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord; //TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl*_LightColor0.rgb;
				o.ambient = ShadeSH9(half4(worldNormal, 1));
				TRANSFER_SHADOW(o)
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//float3 worldPos = mul(unity_ObjectToWorld, i.vertex).xyz;
				// sample the texture
				float sinSq = sin(_Time.y ) * sin(_Time.y );

				//* (_Colour*clamp(sin(_Time.y), 0.0, 1.0) + _Colour2*clamp(sin(_Time.y)*-1.0, 0.0, 1.0)) 
				fixed4 col = tex2D(_MainTex, i.uv) * (_Colour * sinSq + _Colour2 * (1-sinSq));
				fixed shadow = SHADOW_ATTENUATION(i);
				fixed3 lighting = i.diff * shadow + i.ambient;
				col.rgb *= lighting;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;// * _Colour;//(_Colour*sin(_Time.y) + _Colour2*cos(_Time.y) *0.5);
			}
			ENDCG
		}
	}
}
