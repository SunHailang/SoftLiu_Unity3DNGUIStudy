// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SoftLiu/Unlit/BaseStudyUnlitShader"
{
    // 属性
    Properties{
        _MainColor("Main Color", Color) = (1,1,1,1)
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

            // 通过属性 我们可以自定义一些数据
            //float4 _MainColor;/
            uniform float4x4 _mvp;

            // 结构体是编译器为我们组织数据的
            struct v2f
            {
                float4 pos:POSITION;
            };

            //顶点程序
            v2f vert (appdata_base v)
            {
                v2f o;
                // Unity 提供的 MVP 矩阵
                o.pos = UnityObjectToClipPos(v.vertex);
                // 使用自己的 mvp 矩阵
                //o.pos = mul(_mvp, v.vertex);
                return o;
            }
            // 片段程序
            fixed4 frag (v2f i) : COLOR
            {
                return fixed4(1,1,1,1);
            }
            ENDCG
        }
    }
}
