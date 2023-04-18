Shader "Kuwahara/Generalized"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _KernelSize("Kernel Size", Integer) = 1
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            // Adapted from https://en.wikipedia.org/wiki/Kuwahara_filter
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
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;
            float4 _Albedo;
            int _KernelSize;
            static const float PI = 3.14159265f;

            float calcBrightness (float4 color)
            {
                // Luma calculation from https://en.wikipedia.org/wiki/Luma_(video)
                return 0;
            }

            /// Returns:
            /// [0:3] color.
            /// [4] polynomial weight.
            float4 sampleSector (float2 uv, int sector)
            {
                float4 totalColor = 0;

                float totalBrightness = 0;
                float totalBrightnessSquared = 0;

                // Upper/lower bounds
                float lb = (2 * sector - 1) * PI / 8;
                float ub = (2 * sector + 1) * PI / 8;

                int halfSize = _KernelSize / 2.0;
                int n = halfSize * halfSize;
                // n=1;

                for (int r = -halfSize; r <= halfSize; r++)
                {
                    for (int c = -halfSize; c <= halfSize; c++)
                    {
                        float theta = atan2(r + uv.y, c + uv.x);
                        int inSector = lb < theta && theta <= ub;
                        // inSector = 0;

                        // Calculate color at some offset
                        float4 color = tex2D(_MainTex, uv + float2(c,r) * _MainTex_TexelSize.xy) * _Albedo * inSector;
                        totalColor += color;

                        // Calculate brightness
                        float b = calcBrightness(color);

                        // Increment values for running std
                        totalBrightness += b;
                        totalBrightnessSquared += b * b;

                        // n = n + inSector;
                    }
                }

                float mean = totalBrightness / n;
                float std = sqrt(totalBrightnessSquared / n - mean * mean);

                float weight = 1.0f / (1.0f + std);

                return float4(totalColor.rgb, weight);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float weightSum = 0;
                float weights[8];
                float3 colors[8];

                for (int a = 0; a < 8; a++)
                {
                    float4 sectorOutput = sampleSector(i.uv, a);
                    weightSum += sectorOutput[3];
                    weights[a] = sectorOutput[3];
                    colors[a] = sectorOutput.rgb;
                }

                float4 weightedColor = 0;

                for (int a = 0; a < 8; a++)
                {
                    weightedColor += float4(colors[a] * weights[a], 0);
                }
                
                weightedColor /= weightSum;
                weightedColor.a = 1;
                return weightedColor;
            }
            ENDCG
        }
    }
}
