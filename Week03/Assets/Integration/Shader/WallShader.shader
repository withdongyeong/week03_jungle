Shader "Custom/WallShader"
{
    Properties
    {
        _GridSize ("Grid Size", Float) = 10.0 // 격자 크기 조정
        _Range ("Detection Range", Float) = 2.0 // 카메라 감지 거리
        _Color ("Grid Color", Color) = (1, 0, 0, 1) // 빨간색 기본값
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // 투명 블렌딩
            ZWrite Off

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
                float3 worldPos : TEXCOORD1;
            };

            float _GridSize;
            float _Range;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // 월드 좌표 계산
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 카메라와의 거리 계산
                float3 camPos = _WorldSpaceCameraPos;
                float dist = distance(i.worldPos, camPos);

                // 격자 패턴 생성
                float2 grid = frac(i.uv * _GridSize);
                float gridPattern = step(0.9, grid.x) + step(0.9, grid.y);

                // 거리 조건 체크
                if (dist < _Range && gridPattern > 0)
                {
                    return _Color; // 빨간 격자무늬 표시
                }
                return fixed4(0, 0, 0, 0); // 투명
            }
            ENDCG
        }
    }
}