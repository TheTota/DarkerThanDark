

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

            float4 _Color;
            float _MaxRadius;
            int _WavesCount;

            float4 _Origins[100];

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION; 
                float3 worldPos : TEXCOORD1;
            };

            float smoothstep(float a, float b, float x)
            {
                float t = saturate((x - a)/(b - a));
                return t*t*(3.0 - (2.0*t));
            }

            float MultipleFragWave(float3 worldPos) {
                float finalVal = 0;

                for(int i = 0; i < _WavesCount; i++) {
                    float dist = distance(worldPos.xyz, _Origins[i].xyz);
                    float radiusWave = _MaxRadius *  _Origins[i].w;
                    float width = 0.2;    
                    
                    float upper = radiusWave + 0.5 * width;
                    float lower = radiusWave - 0.5 * width;

                    float val = smoothstep(lower, radiusWave, dist) - smoothstep(radiusWave, upper, dist);
           
                    finalVal += max(0, val * (_MaxRadius - radiusWave) / _MaxRadius);
                }

                return finalVal;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {           
                return fixed4(1, 1, 1, 0) * MultipleFragWave(i.worldPos) * _Color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}