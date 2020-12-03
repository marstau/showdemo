
// Shader created with Dite
Shader "YoFi/Model/RTShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _UVOffset("UV Offset",vector) = (1,1,0,0)
        _ShadowCol("Shadow Color",Color) = (0,0,0,0)
    }
    SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            fixed4 _ShadowCol;

            UNITY_INSTANCING_BUFFER_START(Props)
 				UNITY_DEFINE_INSTANCED_PROP(half4, _UVOffset)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
 				
                half4 offset = UNITY_ACCESS_INSTANCED_PROP(Props, _UVOffset);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex) * offset.xy + offset.zw;

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed acol = tex2D(_MainTex, i.texcoord).r;

                UNITY_APPLY_FOG(i.fogCoord, _ShadowCol);
                return fixed4(_ShadowCol.rgb,acol * _ShadowCol.a);
            }
        ENDCG
    }
}

}
