// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SoftLiu/Unlit/DiffuseFragStudyUnlitShader"
{
    Properties
    {
        _SpacularColor("SpacularColor",Color) = (1,1,1,1)
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

            fixed4 _SpacularColor;
            float _Shininess;

            struct v2f
            {
                float4 pos:POSITION;
                float3 normal:TEXCOORD1;
                float4 vertex:COLOR;
            };
            // 光照 = 环境光+漫反射+高光反射
            v2f vert (appdata_base v)
            {
                v2f o;
                // 对顶点进行mvp变换
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.col = float4(1,1,0,1);
                // 法向量 到世界坐标系
                o.normal = v.normal;
                // 灯光向量到世界坐标系
                o.vertex = v.vertex;

                return o;
            }
            // 顶点程序 是基于顶点的计算，每一个顶点都会创建一个顶点计算程序， 
            // 片段程序是光栅化以后的三角形计算是基于像素的处理， 所以片段处理就需要每一个像素的法线和光向量

            // 顶点程序计算光照执行效率高，片段程序计算光照较慢但是细腻平滑
            fixed4 frag (v2f i) : COLOR
            {
                // 环境光
                fixed4 color = UNITY_LIGHTMODEL_AMBIENT;

                // 漫反射光
                float3 N = UnityObjectToWorldNormal(i.normal);
                float3 L = normalize(WorldSpaceLightDir(i.vertex));
                float diffuseScale = saturate(dot(N, L));
                color += _LightColor0 * diffuseScale;

                // 高光反射 Spacular
                // float3 V = normalize(WorldSpaceViewDir(i.vertex));
                // float3 R = 2 * dot(N, L) * N - L;
                // float spacularScale = saturate(dot(R, V));
                // color += _SpacularColor * pow(spacularScale, _Shininess);

                return color;
            }
            ENDCG
        }
    }
}
