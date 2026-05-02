Shader "SafeRun/ColorBlindness"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Mode ("Daltonismo Mode", Int) = 0
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Mode;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 ApplyDaltonismo(fixed4 col, int mode)
            {
                if (mode == 0) return col;

                float r = col.r;
                float g = col.g;
                float b = col.b;

                if (mode == 1)
                {
                    col.r = 0.567 * r + 0.433 * g;
                    col.g = 0.558 * r + 0.442 * g;
                    col.b = 0.242 * r + 0.758 * g + b;
                }
                else if (mode == 2)
                {
                    col.r = 0.625 * r + 0.375 * g;
                    col.g = 0.700 * r + 0.300 * g;
                    col.b = 0.300 * g + b;
                }
                else if (mode == 3)
                {
                    col.r = 0.950 * r + 0.050 * b;
                    col.g = 0.433 * r + 0.567 * g;
                    col.b = 0.475 * g + 0.525 * b;
                }

                return col;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return ApplyDaltonismo(col, _Mode);
            }
            ENDCG
        }
    }
}
