// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "SoftLiu/Textures/Unlit/TextureLightMapUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"


            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // sampler2D unity_Lightmap;
            // float4 unity_LightmapST;

            float tiling_x;
            float tiling_y;
            float offset_x;
            float offset_y;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                o.uv1 = v.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                float3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1));
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= lm;
                return col;
            }
            ENDCG
        }
    }
}
