Shader "SoftLiu/Fusion/Unlit/ColorFusionStudyShader"
{
    Properties
    {
       _MainColor("MainColor", Color) = (1,1,1,1)
       _SecondColor("SecondColor", Color) = (1,1,1,1)
       _Center("Center", Range(-0.7, 0.7)) = 0
       _Radius("Radius", Range(0,0.5)) = 0.2
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

            struct v2f
            {
                float4 pos : POSITION;
                float y : POSITION1;

            };

            float4 _MainColor;
            float4 _SecondColor;
            float _Center;
            float _Radius;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.y = v.vertex.y;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 col;
                // if(i.y>_Center+_Radius)
                // {
                //     col = _MainColor;
                // }else if(i.y>_Center && i.y<_Center+_Radius)
                // {
                //     float d = i.y - _Center;
                //     d = (1 - d/_Radius)-0.5;
                //     d=saturate(d);
                //     col = lerp(_MainColor, _SecondColor, d);
                // } 
                // else if(i.y<_Center && i.y>_Center-_Radius){
                //     float d = _Center - i.y;
                //     d = (1-d/_Radius)-0.5;
                //     d = saturate(d);
                //     col = lerp(_SecondColor,_MainColor, d);
                // }
                // else{
                //     col = _SecondColor;
                // }

                float d = i.y - _Center;
                float s = abs(d);
                d = d/s;

                float f = _Radius == 0? 1 : s / _Radius;
                f = saturate(f);
                d *= f;

                d = d / 2 + 0.5;
                col = lerp(_MainColor, _SecondColor, d);

                // if(i.y>_Center)
                // {
                //     col = _MainColor;
                // }else{
                //     col =_SecondColor;
                // }
                return col;
            }
            ENDCG
        }
    }
}
