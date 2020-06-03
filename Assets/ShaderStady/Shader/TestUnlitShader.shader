Shader "SoftLiu/Unlit/TestUnlitShader"
{
    Properties
    {

    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
            #pragma exclude_renderers d3d11 gles
            // 声明 顶点函数
            #pragma vertex vert
            // 声明 片段函数
            #pragma fragment frag

             void Func(out float4 c);

            struct Input
            {
                float4 pos:POSITION;
                fixed4 cor:COLOR;
            };
            struct Output
            {
                //sample as:
            };
            void vert(Input IN, out float4 pos:POSITION, out fixed4 cor:COLOR)
            {
                pos = IN.pos;
                if(pos.x < 0)
                {
                    cor = fixed4(1, 0, 0, 1);
                }
                else
                {
                    cor = fixed4(0, 1, 0, 1);
                }
                //cor = fixed4(pos.x, pos.y, pos.z, pos.w);
            }

            float Func2(float arr[3])
            {
                float sum = 0;
                for(int i=0; i<arr.Length; i++)
                {
                    sum+=arr[i];
                }
            }
            void frag(inout fixed4 cor:COLOR)
            {
                //cor = pos;
                cor = fixed4(1, 0, 0, 1);
                //return fixed4(1, 0, 0, 1);
                //bool bTrueOrFalse = true;
                //float => float2/float3/float4
                //fixed => fixed2/fixed3/fixed4

                //float2x4 m2x4 = {{0, 0, 1, 1}, {0, 1, 0, 1}};
                //cor = m2x4[0];

                //float arr[4] = {1, 0.5, 0.5, 1};
                //cor = fixed4(arr[0], arr[1], arr[2], arr[1]);
                
                //Func(cor);
                //float arr[] = {0.5, 0.5, 0.5};
                //cor.r = Func2(arr);
            }
            // Cg 参数都是只拷贝
            void Func(out float4 c)
            {
                c = fixed4(0,1,0,1);
            }

            ENDCG
        }
    }
    Fallback "Diffuse"
}
