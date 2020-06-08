// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SoftLiu/Unlit/BaseStudyUnlitShader3"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float4 pos:POSITION;
                fixed4 col:COLOR;
            };

            v2f vert (appdata_base v)
            {
                // float angle = length(v.vertex)*_SinTime.w;
                // float4x4 M={
                //     float4(cos(angle),0,sin(angle),0),
                //     float4(0,1,0,0),
                //     float4(-sin(angle),0,cos(angle),0),
                //     float4(0,0,0,1)
                // };
                // M = mul(UNITY_MATRIX_MVP,M);
                // v.vertex = mul(M,v.vertex);
                // float x = v.vertex.x*cos(angle)+v.vertex.z*sin(angle);
                // float z = v.vertex.z*cos(angle)-v.vertex.x*sin(angle);
                // v.vertex.x = x;
                // v.vertex.z = z;
                float angle = v.vertex.z+_Time.y * 10;
                float4x4 M={
                    float4(sin(angle)/8+0.5,0,0,0),
                    float4(0,1,0,0),
                    float4(0,0,1,0),
                    float4(0,0,0,1)
                };
                v.vertex = mul(M,v.vertex);

                v2f o;
                // v.vertex 是物体的模型坐标
                // 获取物体的屏幕坐标
                 o.pos = UnityObjectToClipPos(v.vertex);
                // o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
                //o.pos = mul(M,v.vertex);
                o.col = fixed4(0, 1,1,1);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.col;
            }
            ENDCG
        }
    }
}
