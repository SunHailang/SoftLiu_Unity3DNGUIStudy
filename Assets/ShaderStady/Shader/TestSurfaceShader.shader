Shader "SoftLiu/Custom/TestSurfaceShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
        _Int("Int", Int) = 2
        _Float("Float", Float) = 1.5
        _Range("Range", Range(0, 5)) = 1.5

        _Color("Color", Color) = (1, 1, 1, 1)
        _Vector("Vector", Vector) = (2, 3, 6, 1)
        _2D("2D", 2D) = ""{}

        _Cube("Cube", Cube) = "white"{}
        _3D("3D", 3D) = "black"{}

    }
    // 语义块
    SubShader
    {
        Tags {"RenderType" = "Opaque"}
        
        CGPROGRAM
        // surface 表示定义的着色器类型为surface，着色器入口的方法为surf(surface function), 光照模型是BlinnPhong
        // #pragma surface surf BlinnPhong

        #pragma surface surf Lambert

        // 

        struct Input
        {
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            // albedo 光源的反射率
            o.Albedo = float4(1, 0, 0, 1);
            o.Emission = float4(0, 1, 0, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
