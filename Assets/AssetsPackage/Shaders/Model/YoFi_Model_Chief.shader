// Shader created with Dite
//武将使用的shader。skin mesh 无法线贴图     
Shader "YoFi/Model/Chief"
{
	Properties
	{		
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
				half4 uv : TEXCOORD0;
				half4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				half4 vertex : SV_POSITION;
				half4 uv : TEXCOORD0;
				UNITY_FOG_COORDS(2)
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;	half4 _MainTex_ST;
			sampler2D _SpecularTex;
			half _Gloss;
			half _light;

			half4 _Diffuse;
			half4 _Specular;
			half4 _huanjingse;

			// 2019.1.11.1f
			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(half4, _LightDir)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _lightCol)
            UNITY_INSTANCING_BUFFER_END(Props)

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o); 

				half4 _LightDirPP =  UNITY_ACCESS_INSTANCED_PROP( Props, _LightDir );

				half3 lightdirnor = normalize(_LightDirPP.xyz);				
				half3 normal = UnityObjectToWorldNormal(v.normal);// v.normal ;


				fixed3 reflectDir = normalize(reflect(-lightdirnor,v.normal));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex));
				half2 ff = max(half2(dot(normal, lightdirnor),dot(viewDir, reflectDir)* _Gloss * 2),0)* _LightDirPP.w;

				
				o.uv = half4(TRANSFORM_TEX(v.uv, _MainTex),ff);
				o.vertex = UnityObjectToClipPos(v.vertex);
			
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
								
				fixed4 _lightColPP =  UNITY_ACCESS_INSTANCED_PROP( Props, _lightCol );

				half4 col = tex2D(_MainTex, i.uv.xy) ;
				half4 scol = tex2D(_SpecularTex, i.uv.xy);
				
				fixed3 diffuse = saturate(_Diffuse.rgb * i.uv.z + _huanjingse);
				fixed3 specular = _Specular.rgb * i.uv.w  * scol; 
 
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

