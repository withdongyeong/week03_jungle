Shader "Custom/MapShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.36, 0.3, 0.24, 1)
        _NoiseStrength("Noise Strength", Range(0, 1)) = 0.3
        _NoiseScale("Noise Scale", Float) = 0.2
        _PlayerPos("Player World Pos", Vector) = (0,0,0,0)
        _FadeStart("Noise Fade Start Distance", Float) = 20
        _FadeEnd("Noise Fade End Distance", Float) = 40
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float3 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _BaseColor;
            float _NoiseStrength;
            float _NoiseScale;
            float4 _PlayerPos;
            float _FadeStart;
            float _FadeEnd;

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = world.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 coord = i.worldPos.xz * _NoiseScale;
                float n = noise(coord);

                float dist = distance(i.worldPos.xz, _PlayerPos.xz);
                float fade = saturate((_FadeEnd - dist) / (_FadeEnd - _FadeStart));

                float factor = 1.0 - _NoiseStrength * n * fade;

                return _BaseColor * factor;
            }
            ENDCG
        }
    }
}
