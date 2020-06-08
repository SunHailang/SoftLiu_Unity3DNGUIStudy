Shader "SoftLiu/Unlit/BaseStudyUnlitShader1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // if(v.vertex.x>0)
                // {
                //     o.col = fixed4(1,0,0,1);
                // }else{
                //     o.col = fixed4(0,1,0,1);
                // }
                if(v.vertex.x==0.5 && v.vertex.y==0.5 && v.vertex.z==0.5)
                {
                  o.col = fixed4(_SinTime.w/2+0.5,_CosTime.w/2+0.5,_SinTime.y/2+0.5,1);
                }else{
                    o.col = fixed4(0,1,0,1);
                }
                // 把模型坐标变换到世界坐标
                // float4 wpos = mul(unity_ObjectToWorld, v.vertex);
                // if(wpos.x>0) {
                //     o.col = fixed4(1,0,0,1);
                // }else{
                //     o.col = fixed4(0,1,0,1);
                // }
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
