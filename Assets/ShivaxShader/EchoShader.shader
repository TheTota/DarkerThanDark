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

            float ComputeDirectionalWaveFilter(float3 worldPos, float3 origin, float3 direction, float angle) {
                float desiredAngle = angle / 2;
                
                float directionAngle = atan2(direction.z, direction.x);
                float fragmentAngle = atan2(worldPos.z - origin.z, worldPos.x - origin.x);

                float endAngle = directionAngle + desiredAngle;
                float startAngle = directionAngle - desiredAngle;
                    
                return step(startAngle, fragmentAngle) - step(endAngle, fragmentAngle);
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