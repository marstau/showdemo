// Shader created with Dite
//士兵部队的批次合并shader。      
Shader "YoFi/Model/Soldier"
{
	Properties
	{
		// [KeywordEnum(UNITY_2017, UNITY_2019)] _Unity ("UNITY 版本", half) = 0		
		[KeywordEnum(All,Right,Left)]_zhenying("阵营 _zhenying",float)= 1

		[Header(AnimMap)]
		_AnimMap ("Anim Map", 2D) ="white" {}
		_moniTime("Frame Time 0",Range(0.0,1.0))= 0.015
		
		[Header(Color)]
		_Diffuse("Diffuse Color",Color) = (1.0,1.0,1.0,1)
		_Specular("Specular Color",Color) = (0.7176471,0.7019608,0.5647059,1)
		_huanjingse("Ambient Color",Color) = (0.5490196,0.4941177,0.4941177,1)

		[Header(Texture)]
		_MainTex ("Texture", 2D) = "white" {}
		_SpecularTex("Specular Texture", 2D) = "white" {}
				
		[Header(Attr)]
		_Gloss("高光范围",float) = 0.66
		_light("亮度",float) = 1.3
		
		[Header(Light)]
		_LightDir("LightDir", vector) = (0.15,1,-0.15,1)
		_lightCol("灯光颜色",Color) = (1,1,1,1)

	}
	SubShader
	{
		Tags { "RenderType" = "Opaque"}
		LOD 100

		Fog{Mode Linear}

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//开启gpu instancing
			#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct appdata
			{
				half3 normal: NORMAL;
				half4 vertex: POSITION;
				half2 uv : TEXCOORD0;
				half4 texcood1 : TEXCOORD1;
				half4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				half4 vertex : SV_POSITION;
				half4 uv : TEXCOORD0;
				half2 valueuv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			
			sampler2D _AnimMap;
			half4 _AnimMap_TexelSize;
			sampler2D _SpecularTex;

			half _Gloss;
			half _light;

			half4 _Diffuse;
			half4 _Specular;
			half4 _huanjingse;

			// 2019.1.11.1f
			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(half, _zhenying)
				UNITY_DEFINE_INSTANCED_PROP(half, _moniTime)

				UNITY_DEFINE_INSTANCED_PROP(half4, _LightDir)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _lightCol)
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


			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				half4 _LightDirPP =  UNITY_ACCESS_INSTANCED_PROP( Props, _LightDir );							
				half animMap_y = UNITY_ACCESS_INSTANCED_PROP(Props, _moniTime);
				half zhenying = UNITY_ACCESS_INSTANCED_PROP(Props, _zhenying);	

				half3 lightdirnor = normalize(_LightDirPP.xyz);				
				
				half4 pos = loadmat(v.texcood1,v.color,v.vertex,animMap_y);
				half3 normal = UnityObjectToWorldNormal(loadmat(v.texcood1,v.color,half4(v.normal, 0),animMap_y).xyz);


				fixed3 reflectDir = normalize(reflect(-lightdirnor,normal));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,pos));
				half dif = max(dot(normal, lightdirnor),0);
				half spe =  max(dot(viewDir, reflectDir)* _Gloss * 2,0);

				half2 uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertex = UnityObjectToClipPos(pos);
				o.valueuv = half2(dif,spe) * _LightDirPP.w;

				o.uv = half4(lerp(uv,uv * half2( 0.5,1 ) + half2(1.5*step(zhenying,1),0),saturate(zhenying)),uv);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				
				fixed4 _lightColPP =  UNITY_ACCESS_INSTANCED_PROP( Props, _lightCol );

				half4 col = tex2D(_MainTex, i.uv.xy) ;
				half4 scol = tex2D(_SpecularTex, i.uv.zw);		
				
				fixed3 diffuse = saturate(_Diffuse.rgb * i.valueuv.x + _huanjingse);
				fixed3 specular = _Specular.rgb * i.valueuv.y  * scol; 
 
				fixed3 _color_te = (diffuse  + specular ) * _light *  _lightColPP.rgb * col.xyz;
				
				fixed4 color = fixed4(_color_te.rgb  , 1.0);

				UNITY_APPLY_FOG(i.fogCoord, color);
				return color;
				
			}
			ENDCG
		}
	}
	FallBack "YoFi/DefaultUnlitShader"
}

