Shader "SoftLiu/Unlit/BaseStudyUnlitShader2"
{
    Properties
    {
        _R("R", Range(0,500))=1
        _OX("OX", Range(-5,5))=0
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

            float _R;
            float _OX;

            v2f vert (appdata_base v)
            {
                float4 wpos = mul(unity_ObjectToWorld, v.vertex);
                float2 xy = wpos.xz;//v.vertex.xz;
                float d = _R - length(xy-float2(_OX,0));
                d = d<0?0:d;
                float height=1;
                float4 uppos = float4(v.vertex.x, height*d,v.vertex.z, v.vertex.w);
                
                v2f o;
                // v.vertex 是物体的模型坐标
                // 获取物体的屏幕坐标
                o.pos = UnityObjectToClipPos(uppos);

                o.col = fixed4(uppos.y, uppos.y,uppos.y,1);

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
