// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SoftLiu/Unlit/DiffuseStudyUnlitShader"
{
    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct v2f
            {
                float4 pos:POSITION;
                fixed4 col:COLOR;
                float3 normal : NORMAL;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                // 对顶点进行mvp变换
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.col = float4(1,1,0,1);
                o.normal = v.normal;

                // 在统一的坐标空间进行光照
                float3 N = normalize(v.normal);
                // 将模型坐标系转到世界坐标系(光的坐标系)
                N = mul(float4(N,0),unity_WorldToObject).xyz;
                N = normalize(N);
                float3 L = normalize(_WorldSpaceLightPos0);
                
                // 将光向量(世界坐标系)转到模型坐标系
                //L = mul(unity_WorldToObject,float4(L,0)).xyz;
                float ndotl =saturate(dot(N,L));
                o.col = _LightColor0*ndotl;

                //"LightMode" = "Vertex"
                //o.col.rgb = ShadeVertexLights(v.vertex, v.normal);

                //"LightMode" = "ForwardBase"
                float3 pos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.col.rgb += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0, 
                                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                        unity_4LightAtten0, pos, N);
                return o;
            }
            // 顶点程序计算光照执行效率高，片段程序计算光照较慢但是细腻平滑
            fixed4 frag (v2f i) : COLOR
            {
                



                // sample the texture
                return i.col + UNITY_LIGHTMODEL_AMBIENT;
            }
            ENDCG
        }
    }
}
