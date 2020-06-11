Shader "SoftLiu/Custom/TestSurfaceShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _MainColor("MainColor", Color) = (1,1,1,1)
        _SecondColor("SecondColor", Color) = (1,1,1,1)
        _Center("Center", Range(-0.7, 0.7)) = 0
        _Radius("Radius", Range(0,0.5)) = 0.2
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        //#pragma surface surf Standard fullforwardshadows
        #pragma surface surf Standard vertex:vert fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
            float2 uv_MainTex;
            float x;
        };

        float4 _MainColor;
        float4 _SecondColor;
        float _Center;
        float _Radius;

        sampler2D _MainTex;

        half _Glossiness;
        half _Metallic;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)



        void vert(inout appdata_full v, out Input o)
        {
            o.uv_MainTex = v.texcoord.xy;
            o.x = v.vertex.x;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            // Albedo comes from a texture tinted by color
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            float d = IN.x - _Center;
            float s = abs(d);
            d = d/s;
            float f = _Radius == 0? 1 : s / _Radius;
            f = saturate(f);
            d *= f;

            d = d / 2 + 0.5;
            o.Albedo *= lerp(_MainColor, _SecondColor, d);

        }
        ENDCG
    }
    FallBack "Diffuse"
}
