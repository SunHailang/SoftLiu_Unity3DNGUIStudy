// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SoftLiu/Unlit/BaseStudyUnlitShader4"
{
    Properties
    {
        _Float("Float", Range(0,2)) = 0.8
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

            float _Float;
            struct v2f
            {
                float4 pos:POSITION;
                fixed4 col:COLOR;
            };

            v2f vert (appdata_base v)
            {
                float up = sin(v.vertex.x)+_Time.y*10;
                
                // A * sin(w*x+t);  简谐运动

                //v.vertex.y +=_Float * sin(-length(v.vertex.xz)+_Time.y);
                v.vertex.y +=_Float * sin((v.vertex.x+v.vertex.z)+_Time.y);
                v.vertex.y +=0.3 * sin((v.vertex.x-v.vertex.z)+_Time.w);
                v2f o;
                // v.vertex 是物体的模型坐标
                // 获取物体的屏幕坐标
                 o.pos = UnityObjectToClipPos(v.vertex);
                // o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
                //o.pos = mul(M,v.vertex);
                if(v.vertex.y>1){
                o.col = fixed4(1, 1,1,1);
                }else
                    o.col = fixed4(0, 0,0,1);

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
