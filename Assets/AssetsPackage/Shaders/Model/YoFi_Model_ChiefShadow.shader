// Shader created with Dite
//武将使用的shader。skin mesh 无法线贴图     
Shader "YoFi/Model/ChiefShadow"
{
	Properties
	{
		_ShadowCol("Color", color) = (0.0,0.0,0.0,0.5)
        _StencilID("Stencil ID", float) = 2
        
        [Header(Light)]
		_LightDir("LightDir", vector) = (0.15,1,-0.15,1)

		_Plane("Plane", vector) = (0,1,0,0.001)
	}
		SubShader
		{
			Tags {"Queue"="Transparent-9" "IgnoreProjector"="True" "RenderType"="Transparent"}
			LOD 100
			Cull Back
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			
		Pass
		{
			Stencil{
                Ref [_StencilID]
                Comp NotEqual
                Pass replace
            }
            offset 0,-1

			CGPROGRAM			
			#pragma vertex vert
			#pragma fragment frag
			//开启gpu instancing
			// #pragma multi_compile_instancing
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex: POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_FOG_COORDS(1)
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			

			half4 _Plane;
			fixed4 _ShadowCol;
			// half4 _LightDir;

			//2019.1.11.1f
			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(half4, _LightDir)
				// UNITY_DEFINE_INSTANCED_PROP(fixed4, _ShadowCol)
            UNITY_INSTANCING_BUFFER_END(Props)

			
			
			v2f vert (appdata v, uint vid : SV_VertexID)
				{
					v2f  o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					//2019.1.11.1f
					// float4 pos = loadmat(v.texcood1,v.color,v.vertex,UNITY_ACCESS_INSTANCED_PROP(Props, _moniTime)); 				
					float3 ld = normalize(UNITY_ACCESS_INSTANCED_PROP(Props, _LightDir).xyz);
					float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					float t = (_Plane.w - dot(worldPos.xyz, _Plane.xyz)) / dot(ld, _Plane.xyz);
					worldPos.xyz = worldPos.xyz + t * ld;
					o.vertex = mul(unity_MatrixVP, worldPos);
					UNITY_TRANSFER_FOG(o,o.vertex);

					return o;
				}
			
				fixed4 frag (v2f i) : SV_Target
				{
					//UNITY_SETUP_INSTANCE_ID(i);
					fixed4 col = _ShadowCol;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
		}
	}
	FallBack "YoFi/DefaultUnlitShader"
}

