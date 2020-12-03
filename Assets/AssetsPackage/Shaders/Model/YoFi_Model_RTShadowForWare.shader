
// Shader created with Dite
Shader "YoFi/Model/RTShadowForWare"
{
	Properties
	{
		_AnimMap ("Anim Map", 2D) ="white" {}
		_moniTime("Frame Time0",Range(0.0,1.0))=0
		//_moniTime2("Frame Time1",Range(0.0,1.0))=0
		//_blendpp("Anim0 Blend To Anim1",Range(0.0,1.0))=0

		_ShadowCol("Color", color) = (0.0,0.0,0.0,0.5)
        //_StencilID("Stencil ID", float) = 2

        
		[Header(Light)]
        _LightDir("Light Dir", vector) = (0,1,1,1)

		_Plane("Plane", vector) = (0,1,0,0.001)


	}
		SubShader
		{
			Tags { "RenderType"="Opaque" }
			LOD 100
		Pass
		{
			//Stencil{
   //             Ref [_StencilID]
   //             Comp NotEqual
   //             Pass replace
   //         }
			Cull Back
            //blend SrcAlpha OneMinusSrcAlpha //DstColor  zero //srcalpha oneminussrcalpha
            //offset 0,-1
			CGPROGRAM			
			#pragma vertex vert
			#pragma fragment frag
			//开启gpu instancing
			#pragma multi_compile_instancing
			//#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex: POSITION;
				float4 texcood1 : TEXCOORD1;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				//float2 shadowcol:TEXCOORD1;
				//UNITY_FOG_COORDS(2)
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			sampler2D _AnimMap;
			float4 _AnimMap_TexelSize;//x == 1/width

			half4 _Plane;
			fixed4 _ShadowCol;
			half4 _LightDir;
			//2019.1.11.1f
			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float, _moniTime)
            UNITY_INSTANCING_BUFFER_END(Props)


			half4 loadmat(half4 verttoboneid,half4 weight,half4 vertex,half frame){
				half4x4 changliang = half4x4(half4(verttoboneid.x * 3 + half4(0.5,1.5,2.5,0))*_AnimMap_TexelSize.x,
											 half4(verttoboneid.y * 3 + half4(0.5,1.5,2.5,0))*_AnimMap_TexelSize.x,
											 half4(verttoboneid.z * 3 + half4(0.5,1.5,2.5,0))*_AnimMap_TexelSize.x,
											 half4(verttoboneid.w * 3 + half4(0.5,1.5,2.5,0))*_AnimMap_TexelSize.x);
				half4 row = half4(0, 0, 0, 1); 

				half4 rowa = mul(half4x4(tex2Dlod(_AnimMap, half4(changliang[0].x, frame, 0, 0)),
										 tex2Dlod(_AnimMap, half4(changliang[0].y, frame, 0, 0)),
										 tex2Dlod(_AnimMap, half4(changliang[0].z, frame, 0, 0)),row),vertex) * weight.x;

				half4 rowb = mul(half4x4(tex2Dlod(_AnimMap, half4(changliang[1].x, frame, 0, 0)),
										 tex2Dlod(_AnimMap, half4(changliang[1].y, frame, 0, 0)),
										 tex2Dlod(_AnimMap, half4(changliang[1].z, frame, 0, 0)),row),vertex) * weight.y;

				half4 rowc = mul(half4x4(tex2Dlod(_AnimMap, half4(changliang[2].x, frame, 0, 0)),
										 tex2Dlod(_AnimMap, half4(changliang[2].y, frame, 0, 0)),
										 tex2Dlod(_AnimMap, half4(changliang[2].z, frame, 0, 0)),row),vertex) * weight.z;

				half4 rowd = mul(half4x4(tex2Dlod(_AnimMap, half4(changliang[3].x, frame, 0, 0)),
										 tex2Dlod(_AnimMap, half4(changliang[3].y, frame, 0, 0)),
										 tex2Dlod(_AnimMap, half4(changliang[3].z, frame, 0, 0)),row),vertex) * weight.w;

				return rowa + rowb + rowc + rowd;
				
			}

			v2f vert (appdata v, uint vid : SV_VertexID)
			{
				v2f  o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				//2019.1.11.1f
				float4 pos = loadmat(v.texcood1,v.color,v.vertex,UNITY_ACCESS_INSTANCED_PROP(Props, _moniTime)); 				
				float3 ld = normalize(  _LightDir.xyz);
				float4 worldPos = mul(unity_ObjectToWorld, pos);
				float t = (0 - dot(worldPos.xyz,fixed3(0,1,0))) / dot( _LightDir.xyz,fixed3(0,1,0));
				worldPos.xyz = worldPos.xyz + t *  _LightDir.xyz;

				o.vertex = mul(unity_MatrixVP, worldPos);
				
				UNITY_TRANSFER_FOG(o,o.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//UNITY_SETUP_INSTANCE_ID(i);
				fixed4 col = _ShadowCol;// * i.shadowcol.x;
				//fixed4 col = saturate( UNITY_ACCESS_INSTANCED_PROP( Props, _ShadowCol )) * i.shadowcol.x;
				// col.a = 0;
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
