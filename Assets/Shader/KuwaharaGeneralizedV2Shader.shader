Shader "Kuwahara/Generalized V2"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _KernelSize("Kernel Size", Integer) = 1

        // The zero crossing is the plane at which we cut the weighting
        // polynomial. All of the polynomial that falls under this plane
        // is treated as zero.
        _ZeroCrossing("Zero Crossing", float) = 8
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            // Adapted from "Anisotropic Kuwahara Filtering with Polynomial
            // Weighting Functions." A copy of the text can be found in the
            // References folder, titled kang-tpcg2010.pdf.
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
            int _KernelRadius;
            float _ZeroCrossing;
            static const float PI = 3.14159265f;

            float calcBrightness (float4 color)
            {
                // Luma calculation from https://en.wikipedia.org/wiki/Luma_(video)
                return 0;
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
                // First, calculate all the variables we can.
                // Radius of the filter area.
                float radius = _KernelRadius;
                // The zero crossing of the polynomial.
                float gamma = _ZeroCrossing;
                // One of the constants for the polynomial weight.
                // Controls how much overlap at the origin.
                float zeta = 2 / radius;
                // The other constant for the polynomial weight.
                // Controls how much overlap at the sides.
                // Looks like n.
                float eta = (zeta + cos(gamma)) / (sin(gamma) * sin(gamma));

                float4 totalColors[8];
                float tcSquared[8];

                ///
                /// Time for calculations
                ///
                // This is blantently stolen from the text btw.

                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        // We will take this offset vector and rotate it to
                        // test the pixels.
                        float2 v = float2(x,y) / radius;
                        float4 color = tex2D(_MainTex, i.uv + float2(x,y) * _MainTex_TexelSize.xy);

                        float weightSum = 0;
                        float weights[8];

                        // The value calculated from our polynomial weight.
                        // The complete equation is (x + zeta) - eta y^2.
                        float z;
                        // Values for calculating z.
                        float vxx;
                        float vyy;

                        // We first start at 0 rotation, and rotate by pi/2
                        // (which is trivial).
                        vxx = zeta - eta * v.x * v.x;
                        vyy = zeta - eta * v.y * v.y;

                        // Rotation = 0
                        z = max(0, v.y + vxx);
                        // We want to square the weight so that it mimics a 
                        // gaussian function.
                        weights[0] = z * z;
                        weightSum += weights[0];

                        // Rotation = pi/2
                        z = max(0, -v.x + vyy);
                        weights[2] = z * z;
                        weightSum += weights[2];

                        // Rotation = pi
                        z = max(0, -v.y + vxx);
                        weights[4] = z * z;
                        weightSum += weights[4];

                        // Rotation = 3pi/2
                        z = max(0, v.x + vyy);
                        weights[6] = z * z;
                        weightSum += weights[6];

                        // Now, we rotate by pi/4 to calculate the other
                        // weights
                        v = sqrt(2) / 2 * float2(v.x - v.y, v.x + v.y);
                        vxx = zeta - eta * v.x * v.x;
                        vyy = zeta - eta * v.y * v.y;

                        // Rotation = pi/4 + 0
                        z = max(0, v.x + vyy);
                        weights[1] = z * z;
                        weightSum += weights[1];

                        // Rotation = pi/4 + pi/2
                        z = max(0, -v.x + vyy);
                        weights[3] = z * z;
                        weightSum += weights[3];

                        // Rotation = pi/4 + pi
                        z = max(0, -v.y + vxx);
                        weights[5] = z * z;
                        weightSum += weights[5];

                        // Rotation = pi/4 + 3pi/2
                        z = max(0, v.x + vyy);
                        weights[7] = z * z;
                        weightSum += weights[7];

                        // Some more calculations with weights.
                        // I'm not really sure what this does exactly.
                        float g = exp(-3.125 * dot(v, v)) / weightSum;

                        for (int k = 0; k < 8; k++)
                        {
                            // Scale each weight by the mystery calculation.
                            float wk = weights[k] * g;
                            // Add to total colors (a value is the weight).
                            totalColors[k] += float4(color.rgb * wk, wk);
                            // Running std calculation.
                            tcSquared[k] += color * color * wk;
                        }
                    }
                }

                float4 finalColor = 0;

                for (int k = 0; k < 8; k++)
                {
                    finalColor += totalColors[k];
                    float std = sqrt(tcSquared[k] / totalColors[k].w -
                        totalColors[k].rgb * totalColors[k].rgb);
                }

                return finalColor / finalColor.w;
            }
            ENDCG
        }
    }
}
