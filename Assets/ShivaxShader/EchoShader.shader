
Shader "Unlit/EchoShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MaxRadius ("Max Radius", float) = 0.5
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

            #include "UnityCG.cginc"

            // Simple wave variable
            float4 _Color;
            float4 _Origin;
            float _MaxRadius;
            float _Speed;

            // Same origin, multiple Waves
            uniform float _Speeds[10];
            float _RadiusWave[10];

            // Different origin, same wave type
            float4 _Origins[255];
            float _SpeedsOrigin[255];

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;   
                float3 worldPos : TEXCOORD1;
            };

            float fragWave(float3 worldPos) {
                float dist = distance(worldPos, _Origin);
                float radiusWave = _MaxRadius * _Speed;
     
                // Circle wave
                float val = 1 - step(dist, radiusWave - 0.05);
                val = step(radiusWave - 0.5, dist) * step(dist, radiusWave) * val;            

                return val * (_MaxRadius - radiusWave ) / _MaxRadius;
            }

            float SuccesivefragWave(float3 worldPos) {
                float dist = distance(worldPos, _Origin);

                float finalVal = 0;

                for(int i = 0; i < 10; i++) {
                    _RadiusWave[i] = _MaxRadius * _Speeds[i];

                    float val = step(dist, _RadiusWave[i]) * step(_RadiusWave[i] - 0.05, dist);

                    finalVal += val * (_MaxRadius - _RadiusWave[i]) / _MaxRadius;
                }

                return finalVal;
            }

            float MultipleFragWave(float3 worldPos) {
                float finalVal = 0;

                for(int i = 0; i < 255; i++) {
                    float dist = distance(worldPos, _Origins[i]);
                    float radiusWave = _MaxRadius * _SpeedsOrigin[i];
;                   
                    float val = step(dist, radiusWave) * step(radiusWave - 0.05, dist);

                    finalVal += max(0, val * (_MaxRadius - radiusWave) / _MaxRadius);
                }

                return finalVal;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {           
                return fixed4(1, 1, 1, 0) * MultipleFragWave(i.worldPos);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
