Shader "Unlit/AntFightingTV"
{
    Properties
    {
        _MainTex ("Texture (optional)", 2D) = "white" {}
        
        _BaseColor ("Color", Color) = (0,0,0,0)
        _NoiseScale ("Noise Scale", Range(10, 800)) = 250
        _NoiseSpeed ("Noise Speed", Range(0, 50)) = 18
        _NoiseContrast ("Noise Contrast", Range(0.1, 6)) = 2.2
        _MixWithTexture ("Mix With Texture", Range(0,1)) = 0
        
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.25
        _ScanlineDensity ("Scanline Density", Range(50, 2000)) = 600
        
        _JitterStrength ("Jitter Strength", Range(0, 0.02)) = 0.003
        _JitterSpeed ("Jitter Speed", Range(0, 30)) = 12
        
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _NoiseScale;
            float _NoiseSpeed;
            float _NoiseContrast;
            float _MixWithTexture;

            float _ScanlineStrength;
            float _ScanlineDensity;

            float _JitterStrength;
            float _JitterSpeed;

            float4 _BaseColor;
            
            float hash21(float2 p)
            {
               
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = _Time.y;
                
                float jitter = (hash21(float2(floor(i.uv.y * 200.0), floor(t * _JitterSpeed))) - 0.5) * 2.0;
                float2 uv = i.uv + float2(jitter * _JitterStrength, 0);
                
                float2 p = uv * _NoiseScale;
                
                float2 anim = float2(t * _NoiseSpeed, -t * (_NoiseSpeed * 0.73));
                float n = hash21(floor(p + anim));
                
                n = saturate((n - 0.5) * _NoiseContrast + 0.5);
                float bw = step(0.5, n); 
                
                float scan = sin((uv.y + t * 0.02) * _ScanlineDensity) * 0.5 + 0.5;
                bw = lerp(bw, bw * scan, _ScanlineStrength);
                
                float texLuma = 1.0;
                if (_MixWithTexture > 0.0001)
                {
                    float3 tex = tex2D(_MainTex, uv).rgb * _BaseColor;
                    texLuma = dot(tex, float3(0.299, 0.587, 0.114));
                }

                float finalLuma = lerp(bw, texLuma, _MixWithTexture);
                fixed4 col = fixed4(finalLuma, finalLuma, finalLuma, 1);

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDHLSL
        }
    }
}