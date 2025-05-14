Shader "UI/SoftEdge"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeSoftness ("Edge Softness", Range(0.0, 1.0)) = 0.1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EdgeSoftness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 distToCenter = abs(i.uv - 0.5);
                float alphaFade = smoothstep(0.5, 0.5 - _EdgeSoftness, max(distToCenter.x, distToCenter.y));
                col.a *= alphaFade;
                return col;
            }
            ENDCG
        }
    }
}
