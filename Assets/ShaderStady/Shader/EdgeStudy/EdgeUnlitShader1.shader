Shader "SoftLiu/Edge/Unlit/EdgeUnlitShader1"
{
    Properties
    {
        _MainColor("MainColor", Color) = (1,1,1,1)
        _Scale("Scale", Range(0, 5)) = 0.2
        _Other("Other", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite OFF

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _MainColor;
            float _Scale;
            float _Other;

            struct v2f
            {
                float4 pos : POSITION;
                float3 normal : NORMAL;
                float4 vertex : POSITION1;
            };

            v2f vert (appdata_base v)
            {
                v.vertex.xyz += v.normal * _Other;
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                o.normal = v.normal;

                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                // 将法向量转换成世界坐标系下
                float3 N = UnityObjectToWorldNormal(i.normal);
                // 获取顶点到摄像机的向量 （摄像机位置坐标 - 顶点的位置坐标）
                // float3 wpos = mul(unity_ObjectToWorld, i.vertex).xyz;
                // float3 V = normalize(_WorldSpaceCameraPos.xyz - wpos);
                float3 V = normalize(WorldSpaceViewDir(i.vertex));
                // 计算法向量 和 顶点到摄像机的向量的 Dot
                float bright = saturate(dot(N, V));
                // 衰减 
                bright = pow(bright, _Scale);
                _MainColor.a *= bright;
                return _MainColor;
            }
            ENDCG
        }
        // ====================================

        Pass
        {
            BlendOp RevSub
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite OFF

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : POSITION;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                return fixed4(1,1,1,1);
            }
            ENDCG
        }

        // ====================================
        Pass
        {
            //Blend Zero One
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite OFF

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _MainColor;
            float _Scale;
            float _Other;

            struct v2f
            {
                float4 pos : POSITION;
                float3 normal : NORMAL;
                float4 vertex : POSITION1;
            };

            v2f vert (appdata_base v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                o.normal = v.normal;

                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                // 将法向量转换成世界坐标系下
                float3 N = UnityObjectToWorldNormal(i.normal);
                // 获取顶点到摄像机的向量 （摄像机位置坐标 - 顶点的位置坐标）
                // float3 wpos = mul(unity_ObjectToWorld, i.vertex).xyz;
                // float3 V = normalize(_WorldSpaceCameraPos.xyz - wpos);
                float3 V = normalize(WorldSpaceViewDir(i.vertex));
                // 计算法向量 和 顶点到摄像机的向量的 Dot
                float bright = 1 - saturate(dot(N, V));
                // 衰减 
                bright = pow(bright, _Scale);
                return _MainColor * bright;
            }
            ENDCG
        }
    }
}
