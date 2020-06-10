// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SoftLiu/Unlit/SpecularStudyUnlitShader"
{
    Properties
    {
        _Spacular("Spacular",Color) = (1,1,1,1)
        _Shininess("Shininess",Range(1,64)) = 8
    }
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

            fixed4 _Spacular;
            float _Shininess;

            struct v2f
            {
                float4 pos:POSITION;
                fixed4 col:COLOR;
                float3 normal : NORMAL;
            };
            // 光照 = 环境光+漫反射+高光反射
            v2f vert (appdata_base v)
            {
                v2f o;
                // 对顶点进行mvp变换
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.col = float4(1,1,0,1);
                o.normal = v.normal;

                // 在统一的坐标空间进行光照
                // float3 N = normalize(v.normal);
                // 将模型坐标系转到世界坐标系(光的坐标系)
                // N = mul(float4(N,0),unity_WorldToObject).xyz;
                float3 N = UnityObjectToWorldNormal(v.normal);
                float3 L = normalize(WorldSpaceLightDir(v.vertex));
                float3 V = normalize(WorldSpaceViewDir(v.vertex));
                // 环境光
                o.col = UNITY_LIGHTMODEL_AMBIENT;

                // 将光向量(世界坐标系)转到模型坐标系
                //L = mul(unity_WorldToObject,float4(L,0)).xyz;
                // diffuse color 漫反射光
                float ndotl =saturate(dot(N,L));
                o.col += _LightColor0*ndotl;

                //"LightMode" = "Vertex"
                //o.col.rgb = ShadeVertexLights(v.vertex, v.normal);
                
                //"LightMode" = "ForwardBase"  点光源
                float3 pos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.col.rgb += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0, 
                                        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                        unity_4LightAtten0, pos, N);

                // specular color
                // float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // float3 I = wpos -  _WorldSpaceLightPos0;
                // 镜面高光反射
                // float3 R = reflect(I, N);
                //float3 R = 2*dot(N,L)*N-L;
                // R = normalize(R);
                float3 H = normalize(L+V);

                float spacularScale = pow(saturate(dot(H,N)),_Shininess);
                o.col += _Spacular*spacularScale;

                return o;
            }
            // 顶点程序计算光照执行效率高，片段程序计算光照较慢但是细腻平滑
            fixed4 frag (v2f i) : COLOR
            {
                return i.col;
            }
            ENDCG
        }
    }
}
