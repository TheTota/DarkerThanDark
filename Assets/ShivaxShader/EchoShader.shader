Shader "Unlit/EchoShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
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

            int _WavesCount;

            // Each float4 origin contains the distance pourcentage in the w coordinate.
            float4 _Origins[100];
            float _Radius[100];
            fixed4 _Colors[100];

            // Configurable parameters
            float _WaveColorIntensity;
            float _WaveRadiusWidth;
            float _WaveTrailWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float ComputeWaveTrail(float dist, float waveDistance, float innerCirclePercent) {
                float waveTrail = step(dist, waveDistance);
                float innerCircleDistance = dist - innerCirclePercent * waveDistance;
                float waveTrailWidth =  innerCircleDistance / waveDistance;

                return waveTrail * waveTrailWidth;
            }

            fixed4 MultipleFragWave(float3 worldPos) {
                fixed4 finalVal = fixed4(0,0,0,0);

                for(int i = 0; i < _WavesCount; i++) {
                    float dist = distance(worldPos.xyz, _Origins[i].xyz);
                    float circleWave = _Radius[i] *  _Origins[i].w; 
                    float circleWidth = _WaveRadiusWidth;    
                    
                    float upper = circleWave + 0.5 * circleWidth;
                    float lower = circleWave - 0.5 * circleWidth;

                    float val = smoothstep(lower, circleWave, dist) - smoothstep(circleWave, upper, dist);

                    // Compute wave trail and width
                    float waveTrail = ComputeWaveTrail(dist, circleWave, 1 - _WaveTrailWidth);

                    float fadeFactor = (_Radius[i] - circleWave) / _Radius[i];
                    
                    finalVal = 1 * max(finalVal, _Colors[i] * (_WaveColorIntensity * val + waveTrail) * fadeFactor);
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
                fixed4 color = MultipleFragWave(i.worldPos);

                return color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}