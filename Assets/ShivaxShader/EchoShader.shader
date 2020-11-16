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

            #define M_PI 3.1415926535897932384626433832795

            int _WavesCount;

            // Each float4 origin contains the distance pourcentage in the w coordinate.
            float4 _Origins[100];
            float _Radius[100];
            fixed4 _Colors[100];

            // directional wave
            float3 _Directions[100];
            float _Angles[100];

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

            // Normalize angle in [0, 2pi] range
            float NormalizeAngle(float angle) {
                return (angle + 2 * M_PI) % (2 * M_PI);
            }

            float ComputeDirectionalWaveFilter(float3 worldPos, float3 origin, float3 direction, float angle) {
                float desiredAngle = angle / 2;
                
                float directionAngle = NormalizeAngle(atan2(direction.z, direction.x));
                float fragmentAngle = NormalizeAngle(atan2(worldPos.z - origin.z, worldPos.x - origin.x));

                float endAngle = directionAngle + desiredAngle;
                float startAngle = directionAngle - desiredAngle; 
                
                float normalizedEndAngle = NormalizeAngle(endAngle);
                float normalizedStartAngle = NormalizeAngle(startAngle);

                // Some ugly calculations to get the correct angle to which dial of the circle the fragment is located without branching
                float t1 = step(fragmentAngle, normalizedEndAngle) * (step(2 * M_PI, endAngle) + step(startAngle, 0)) 
                         + step(startAngle, fragmentAngle) * step(endAngle, 2 * M_PI) * step(0, startAngle);

                float t2 = step(normalizedStartAngle, fragmentAngle) * ( step(2 * M_PI, endAngle) + step(startAngle, 0)) 
                         - step(endAngle, fragmentAngle) * step(endAngle, 2 * M_PI) * step(0, startAngle);           

                return t1 + t2;
            }

            float GetDirectionalWaveFilter(float3 worldPos, float3 origin, float3 direction, float angle) {
                return angle > 0 ? ComputeDirectionalWaveFilter(worldPos, origin, direction, angle) : 1;                
            }

            fixed4 MultipleFragWave(float3 worldPos) {
                fixed4 finalVal = fixed4(0,0,0,0);

                for(int i = 0; i < _WavesCount; i++) {
                    float dist = distance(worldPos.xyz, _Origins[i].xyz);
                    float circleWave = _Radius[i] *  _Origins[i].w; 
                    float circleWidth = _WaveRadiusWidth;  
                    
                    // calculate angle          
                    float upper = circleWave + 0.5 * circleWidth;
                    float lower = circleWave - 0.5 * circleWidth;

                    float val = smoothstep(lower, circleWave, dist) - smoothstep(circleWave, upper, dist);

                    // compute filter if wave is directional
                    float directionalWaveFilter = GetDirectionalWaveFilter(worldPos, _Origins[i], _Directions[i], _Angles[i]);
                 
                    // Compute wave trail and width
                    float waveTrail = ComputeWaveTrail(dist, circleWave, 1 - _WaveTrailWidth);

                    float fadeFactor = (_Radius[i] - circleWave) / _Radius[i];
                    
                    finalVal = max(finalVal, _Colors[i] * directionalWaveFilter * (_WaveColorIntensity * val + waveTrail) * fadeFactor);
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