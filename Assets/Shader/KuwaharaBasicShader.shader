Shader "Kuwahara/Basic"
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

            float calcBrightness (float4 color)
            {
                // Luma calculation from https://en.wikipedia.org/wiki/Luma_(video)
                return color.r * 0.2126 + color.g * 0.7152 + color.b * 0.0722;
            }

            /// Returns:
            /// [0:3] color.
            /// [4] std
            float4 sampleNeighbors (float2 uv, int offX, int offY)
            {
                // Running STD calculation from https://math.stackexchange.com/a/2148949
                float totalBrightness = 0;
                float totalBrightnessSquared = 0;
                float4 totalColor = 0;

                // Window dimentions
                int wDim = _KernelSize * 2 + 1;
                // Subwindow dimentions
                int swDim = int(ceil(wDim / 2.0f));
                int n = swDim * swDim;

                for (int r = _KernelSize * (offY - 1); r <= _KernelSize * offY; r++)
                {
                    for (int c = _KernelSize * (offX - 1); c <= _KernelSize * offX; c++)
                    {
                        // Calculate color at some offset
                        float4 color = tex2D(_MainTex, uv + float2(r,c) * _MainTex_TexelSize.xy) * _Albedo;
                        totalColor += color;

                        // Calculate brightness
                        float b = calcBrightness(color);

                        // Increment values for running std
                        totalBrightness += b;
                        totalBrightnessSquared += b * b;
                    }
                }

                float mean = totalBrightness / n;
                float std = sqrt(totalBrightnessSquared / n - mean * mean);

                float4 avgColor = totalColor / n;
                return float4(avgColor.rgb, std);
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
                // sample the texture
                float4 q1 = sampleNeighbors(i.uv, 1, 1);
                float4 q2 = sampleNeighbors(i.uv, 0, 1);
                float4 q3 = sampleNeighbors(i.uv, 0, 0);
                float4 q4 = sampleNeighbors(i.uv, 1, 0);

                // Get stds, then mask the output with them.
                float minStd = min(q1.a, min(q2.a, min(q3.a, q4.a)));
                int4 mask = int4(q1.a == minStd, q2.a == minStd, q3.a == minStd, q4.a == minStd);
                // int4 mask = 1;

                return q1 * mask[0] + q2 * mask[1] + q3 * mask[2] + q4 * mask[3];
            }
            ENDCG
        }
    }
}
