Shader "Kuwahara/Generalized V3"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        
        // Radius of kernel.
        _Radius("Kernel Radius", range(1, 64)) = 6

        // Scale of the zero-crossing, which is the plane at which we cut the
        // weighting polynomial. All of the polynomial that falls under this
        // plane is treated as zero.
        _Scale("Zero-Crossing Coefficient", range(1, 2)) = 1.5

        // Determines how blurry the image is.
        _Sharpness("Sharpness", range(0, 10)) = 10

        // Determines how in focus the image is.
        _Focus("Focus", range(0, 40)) = 10
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
            int _Radius;
            float _Scale, _Sharpness, _Focus;
            static const float PI = 3.14159265f;

            float calcBrightness (float3 color)
            {
                // Luma calculation from https://en.wikipedia.org/wiki/Luma_(video)
                return color.r * 0.2126 + color.g * 0.7152 + color.b * 0.0722;
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
                float radius = _Radius;
                // Scale of the zero-crossing (offset is bc rounding can make
                // the value freak out at low levels).
                float scale = _Scale + 0.05;
                // The zero crossing of the polynomial.
                float gamma = scale * PI / 8;
                // One of the constants for the polynomial weight.
                // Controls how much overlap at the origin.
                float zeta = 2.0 / radius;
                // The other constant for the polynomial weight.
                // Controls how much overlap at the sides.
                // Looks like n.
                float eta = (zeta + cos(gamma)) / (sin(gamma) * sin(gamma));

                float4 totalColors[8];
                float3 tcSquared[8];

                for (int itr1 = 0; itr1 < 8; itr1++)
                {
                    totalColors[itr1] = 0.0;
                    tcSquared[itr1] = 0.0;
                }

                ///
                /// Time for calculations
                ///
                // Most of this code is from the text

                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
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
                        v = sqrt(2.0) / 2.0 * float2(v.x - v.y, v.x + v.y);
                        vxx = zeta - eta * v.x * v.x;
                        vyy = zeta - eta * v.y * v.y;

                        // Rotation = pi/4 + 0
                        z = max(0, v.y + vxx);
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

                float4 finalColor = 0.0;

                for (int k = 0; k < 8; k++)
                {
                    totalColors[k].rgb /= totalColors[k].w;
                    float3 std = sqrt(abs(tcSquared[k] / totalColors[k].w
                        - totalColors[k].rgb * totalColors[k].rgb));
                    float b = calcBrightness(std);

                    float weight = 1.0 / (1.0 + pow(_Focus * b, _Sharpness));

                    finalColor += float4(totalColors[k].rgb * weight, weight);
                }

                finalColor = float4((finalColor / finalColor.w).rgb, 1);
                return finalColor;
            }
            ENDCG
        }
    }
}
