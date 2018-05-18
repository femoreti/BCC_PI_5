// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/World3D"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_climb("Curve", Range(-1,1)) = 0
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			const float _climb = -0.001;
			
			v2f vert (appdata v)
			{
				v2f o;

				// Transform the vertex coordinates from model space into world space
				float4 vv = mul(unity_ObjectToWorld, v.vertex);

				// Now adjust the coordinates to be relative to the camera position
				vv.xyz -= _WorldSpaceCameraPos.xyz;

				// Reduce the y coordinate (i.e. lower the "height") of each vertex based
				// on the square of the distance from the camera in the z axis, multiplied
				// by the chosen curvature factor
				vv = float4(0.0f, (vv.z * vv.z) * (_climb), 0.0f, 0.0f);

				// Now apply the offset back to the vertices in model space
				v.vertex += mul(unity_WorldToObject, vv);

				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.vertex.y -= pow(o.vertex.z,2);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
