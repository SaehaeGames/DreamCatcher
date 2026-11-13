
Shader "Custom/WhiteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineSize ("Outline Size", Range(0, 10)) = 2
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 _OutlineColor;
            float _OutlineSize;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, i.uv);

                float2 texel = _MainTex_TexelSize.xy * _OutlineSize;

                float outlineAlpha = 0;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        float2 offset = float2(x, y) * texel;

                        fixed alpha = tex2D(
                            _MainTex,
                            i.uv + offset
                        ).a;

                        outlineAlpha = max(outlineAlpha, alpha);
                    }
                }

                float outline = outlineAlpha * (1 - original.a);

                fixed4 result = lerp(
                    _OutlineColor,
                    original,
                    original.a
                );

                result.a = max(original.a, outline * _OutlineColor.a);

                result.rgb *= i.color.rgb;
                result.a *= i.color.a;

                return result;
            }

            ENDCG
        }
    }
}
