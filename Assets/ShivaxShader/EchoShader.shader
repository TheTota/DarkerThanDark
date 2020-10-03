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

            fixed intensity(fixed4 color) {
                return (color.x + color.y + color.z) / 3.0;
            }

            fixed4 MultipleFragWave(float3 worldPos) {
                fixed4 finalVal = fixed4(0,0,0,0);
                fixed4 color = fixed4(0,0,0,0);

                for(int i = 0; i < _WavesCount; i++) {
                    float dist = distance(worldPos.xyz, _Origins[i].xyz);
                    float radiusWave = _Radius[i] *  _Origins[i].w;
                    float width = 0.2;    
                    
                    float upper = radiusWave + 0.5 * width;
                    float lower = radiusWave - 0.5 * width;

                    float val = smoothstep(lower, radiusWave, dist) - smoothstep(radiusWave, upper, dist);
                    val *= (_Radius[i] - radiusWave) / _Radius[i];
                    val *= 2 * pow(val, 4);

                    color = _Colors[i] * step(intensity(_Colors[i]), intensity(color));
                    
                    finalVal += val * _Colors[i];
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
                return MultipleFragWave(i.worldPos);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}