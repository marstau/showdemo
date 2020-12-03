
Shader "YoFi/Map/TerrainOpt"
{
Properties
	{
		// _MaskTex ("Mask Tex", 2D) = "white" {}
		_MainTex ("Main Tex", 2D) = "white" {}
		// _LightMapTex ("Light Tex", 2D) = "white" {}		
		_compensate("接缝补偿",float) = 10
		//_hightbuchang("Hight补偿",Range(0.9,1)) = 1
		[Space()]
		_tiledPP1("tiole1 ",vector) = (1,1,1,1)
		_tiledPP2("tiole2 ",vector) = (1,1,1,1)
		// _tiledPP3("tiole3 ",vector) = (1,1,1,1)
        // _tiledPP4("tiole4 ",vector) = (1,1,1,1)
		// _lightmapuv("lightmapuv ",vector) = (1,1,0,0)

		_buchangse("补偿色",color) = (0.5,0.5,0.5,1)
	}

	SubShader
	{
        Tags { "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON

			#include "UnityCG.cginc"
			
			// uniform sampler2D _MaskTex;
			uniform sampler2D _MainTex;

			uniform half4 _MainTex_ST;
			half4 _MainTex_TexelSize;
			// half4 _MaskTex_TexelSize;

			UNITY_INSTANCING_BUFFER_START( Props )
				UNITY_DEFINE_INSTANCED_PROP( half, _compensate)   
				UNITY_DEFINE_INSTANCED_PROP( half4, _buchangse)               
				UNITY_DEFINE_INSTANCED_PROP( half4, _tiledPP1)
				UNITY_DEFINE_INSTANCED_PROP( half4, _tiledPP2)
				// UNITY_DEFINE_INSTANCED_PROP( half4, _tiledPP3)
                // UNITY_DEFINE_INSTANCED_PROP( half4, _tiledPP4)
            UNITY_INSTANCING_BUFFER_END( Props )

			struct a2v
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
                half2 texcoord1 : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				half4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;                
                #ifndef LIGHTMAP_OFF
                half2 uvLM : TEXCOORD4;
                #endif

                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			half2 FromFrameGetUv(half2 uv ,half2 frame,half2 Tiling,half2 Tilingsize,half2 TexelSize,half buchang )
			{
				// 最开始的算法
				// 	return  (fmod(( (uv  + fixed2(1,-1) ) * sr.zw * Tiling + half2(0,0) ), sr.zw) + fmod(half2(frame ,-floor(frame * sr.z)) , sr.xy) * sr.zw);

				// half ff = _hightpow;
				// half aa = 512;
				// half2 bb =  frac(uv   * Tiling) * (aa - _hightpow) + (aa * half2(2,2) + _hightpow * 0.5);
				// return bb ;// _MainTex_TexelSize.xy;

				// UV 
				// frame [0,0] [0,1]
				// Tiling 重复度
				// Tilingsize 切片的大小
				// TexelSize =  _MainTex_TexelSize.xy
				// buchang 接缝补偿

				// fixed4 outcol = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(2,5),5,half2(256,256),_MainTex_TexelSize,_hightpow));			

				return (frac(uv * Tiling) * (Tilingsize - buchang) + (Tilingsize * frame.xy + buchang * 0.5)) * TexelSize;
				
			}			
			
			v2f vert (a2v v)
			{
                v2f o;
				
                UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				//half hight =  UNITY_ACCESS_INSTANCED_PROP( Props, _hightpow );
				//half hightBC =  UNITY_ACCESS_INSTANCED_PROP( Props, _hightbuchang );
				
				//half4 pos = v.vertex + half4(0,hight * (tex2Dlod(_MaskTex,half4(FromFrameGetUv(v.uv*hightBC+(1-hightBC) * 0.5 ,half4(2,2,0.5,0.5),3,1) ,0,0)).x * 2),0,0);

                #ifndef LIGHTMAP_OFF
                o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif

				o.pos = UnityObjectToClipPos(v.vertex);                
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);// + fixed2(1,-1)) * _selfrect.zw ;
                UNITY_TRANSFER_FOG(o,o.pos);
                
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{ 
				UNITY_SETUP_INSTANCE_ID( i );
				half _hightpow = UNITY_ACCESS_INSTANCED_PROP( Props, _compensate ); 
				half4 ttpp1 =  UNITY_ACCESS_INSTANCED_PROP( Props, _tiledPP1 );
				half4 ttpp2 =  UNITY_ACCESS_INSTANCED_PROP( Props, _tiledPP2 );
				// half4 ttpp3 =  UNITY_ACCESS_INSTANCED_PROP( Props, _tiledPP3 );	
                // half4 ttpp4 =  UNITY_ACCESS_INSTANCED_PROP( Props, _tiledPP4 );

				fixed4 m1 = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(0,1),1,half2(512,512),_MainTex_TexelSize,_hightpow));
				fixed4 m2 = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(1,1),1,half2(512,512),_MainTex_TexelSize,_hightpow));
				// fixed4 m3 = tex2D(_MaskTex, FromFrameGetUv(i.uv,half2(0,1),1,half2(512,512),_MaskTex_TexelSize,0));
                // fixed4 m4 = tex2D(_MaskTex, FromFrameGetUv(i.uv,half2(1,1),1,half2(512,512),_MaskTex_TexelSize,0));

				// m1 = smoothstep(0, 1, m1);
				// m2 = smoothstep(0, 1, m2);
			
				fixed4 c1R = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(0,0),ttpp1.x,half2(256,256),_MainTex_TexelSize,_hightpow));
				fixed4 c1G = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(1,0),ttpp1.y,half2(256,256),_MainTex_TexelSize,_hightpow));
				fixed4 c1B = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(2,0),ttpp1.z,half2(256,256),_MainTex_TexelSize,_hightpow));
				fixed4 c1A = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(3,0),ttpp1.w,half2(256,256),_MainTex_TexelSize,_hightpow));			
				 
				fixed4 c2R = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(0,1),ttpp2.x,half2(256,256),_MainTex_TexelSize,_hightpow));
				fixed4 c2G = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(1,1),ttpp2.y,half2(256,256),_MainTex_TexelSize,_hightpow));
				fixed4 c2B = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(2,1),ttpp2.z,half2(256,256),_MainTex_TexelSize,_hightpow));
				fixed4 c2A = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(3,1),ttpp2.w,half2(256,256),_MainTex_TexelSize,_hightpow));

				// fixed4 c3R = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(0,2),ttpp3.x,half2(256,256),_MainTex_TexelSize,_hightpow));
				// fixed4 c3G = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(1,2),ttpp3.y,half2(256,256),_MainTex_TexelSize,_hightpow));
				// fixed4 c3B = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(2,2),_tiledPP3.z,half2(256,256),_MainTex_TexelSize,_hightpow));
				// fixed4 c3A = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(3,2),ttpp3.w,half2(256,256),_MainTex_TexelSize,_hightpow));

                // fixed4 c4R = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(0,3),ttpp4,half2(256,256),_MainTex_TexelSize,_hightpow));
				// fixed4 c4G = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(1,3),ttpp4,half2(256,256),_MainTex_TexelSize,_hightpow));
				// fixed4 c4B = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(2,3),ttpp4,half2(256,256),_MainTex_TexelSize,_hightpow));
				// fixed4 c4A = tex2D(_MainTex, FromFrameGetUv(i.uv,half2(3,3),ttpp4,half2(256,256),_MainTex_TexelSize,_hightpow));

				fixed4 outcol = c1R * m1.r + c1G * m1.g + c1B * m1.b +  c1A * m1.a 
								+ c2R * m2.r + c2G * m2.g + c2B * m2.b +  c2A * m2.a;
								// + c3R * m3.r + c3G * m3.g + c3B * m3.b +  c3A * m3.a
                                // + c4R * m4.r + c4G * m4.g + c4B * m4.b +  c4A * m4.a;

                #ifndef LIGHTMAP_OFF
                fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));
                outcol.rgb*=lm;
                #endif
				outcol *= UNITY_ACCESS_INSTANCED_PROP( Props, _buchangse ) * 2;
				// half4 luv = UNITY_ACCESS_INSTANCED_PROP( Props, _lightmapuv );
				// outcol = outcol * tex2D(_LightMapTex,i.uv * luv.xy + luv.zw);//  lightmapcol; _LightMapTex
                UNITY_APPLY_FOG(i.fogCoord, outcol);
				return fixed4(outcol.rgb,1) ;
			}
			ENDCG
		}
	}
	Fallback "YoFi/DefaultUnlitShader"
}
