Shader "Universal Render Pipeline/Custom/WaterProcUV"
{
    Properties
    {
        _Color ("Color base", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.85
        _Metallic   ("Metallic",   Range(0,1)) = 0.0

        // Procedural Noise (UV)
        _NoiseScale      ("Noise Scale (UV)", Float) = 1.0
        _NoiseOctaves    ("Noise Octaves (1-6)", Range(1,6)) = 4
        _NoiseLacunarity ("Lacunarity", Float) = 2.0
        _NoiseGain       ("Gain", Float) = 0.5
        _NoisePower      ("Noise Power", Range(0.5,4.0)) = 1.4
        _NoiseSpeed      ("Noise Speed (UV XY)", Vector) = (0.2, 0.1, 0, 0)
        _Seed            ("Seed", Float) = 0.0
        _NoiseUVTiling   ("Noise UV Tiling (xy), Offset (zw)", Vector) = (1,1,0,0)

        // Displace/Normals
        _DisplaceAmp     ("Amplitud desplazamiento", Range(0,0.5)) = 0.05
        _NormalStrength  ("Fuerza normal", Range(0,4)) = 1.5
        _NormalDelta     ("Delta derivadas", Range(0.001, 0.2)) = 0.02

        // Fresnel
        _FresnelPower    ("Fresnel Power", Range(0.5,8.0)) = 3.0
        _FresnelStrength ("Fresnel Strength", Range(0,2)) = 0.8
        _EdgeColor       ("Fresnel/Edge Color", Color) = (0.75, 0.9, 1.0, 1.0)
        _BaseAlpha       ("Alpha base", Range(0,1)) = 0.5

        // Intersección (Depth)
        _IntersectionThickness ("Grosor intersección (m aprox)", Range(0.001, 1.0)) = 0.2
        _IntersectionStrength  ("Fuerza intersección", Range(0,3)) = 1.2
        _FoamContactColor      ("Color espuma contacto", Color) = (1,1,1,1)

        // Foam de crestas
        _FoamColor        ("Color espuma crestas", Color) = (1,1,1,1)
        _FoamThreshold    ("Umbral altura (0-1)", Range(0,1)) = 0.65
        _FoamSlopeFactor  ("Peso pendiente", Range(0,4)) = 1.0
        _FoamSharpness    ("Dureza/contraste", Range(0.5,8.0)) = 2.5
        _FoamIntensity    ("Intensidad crestas", Range(0,3)) = 1.2
        _FoamDetailScale  ("Detalle (2ª capa)", Float) = 2.0
        _FoamDetailSpeed  ("Vel Detalle (UV XY)", Vector) = (0.35, 0.2, 0, 0)
    }

    SubShader
    {
        Tags{
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        LOD 350

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Name "ForwardLit"
            Tags{ "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            // URP variants
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float  _Glossiness;
                float  _Metallic;

                float  _NoiseScale, _NoiseLacunarity, _NoiseGain, _NoisePower;
                float4 _NoiseSpeed; // xy
                float  _Seed, _NoiseOctaves;
                float4 _NoiseUVTiling; // xy tiling, zw offset

                float  _DisplaceAmp, _NormalStrength, _NormalDelta;

                float  _FresnelPower, _FresnelStrength, _BaseAlpha;
                float4 _EdgeColor;

                float  _IntersectionThickness, _IntersectionStrength;
                float4 _FoamContactColor;

                float4 _FoamColor;
                float  _FoamThreshold, _FoamSlopeFactor, _FoamSharpness, _FoamIntensity;
                float  _FoamDetailScale;
                float4 _FoamDetailSpeed; // xy
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 tangentWS  : TEXCOORD2;
                float3 bitanWS    : TEXCOORD3;
                float2 uv         : TEXCOORD4;
                float3 viewDirWS  : TEXCOORD5;
                float4 screenPos  : TEXCOORD6;
                float4 shadowCoord: TEXCOORD7;   // <— para sombras
                float  fogFactor  : TEXCOORD8;   // <— para MixFog
            };

            // ---------------- Value noise + FBM ----------------
            float hash21(float2 p, float seed)
            {
                return frac(sin(dot(p, float2(12.9898,78.233)) + seed) * 43758.5453123);
            }

            float vnoise(float2 p, float seed)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f*f*f*(f*(f*6 - 15) + 10);

                float a = hash21(i + float2(0,0), seed);
                float b = hash21(i + float2(1,0), seed);
                float c = hash21(i + float2(0,1), seed);
                float d = hash21(i + float2(1,1), seed);

                float ab = lerp(a, b, u.x);
                float cd = lerp(c, d, u.x);
                return lerp(ab, cd, u.y);
            }

            int ClampOctaves(float v)
            {
                int oct = (int)floor(v + 0.5);
                oct = clamp(oct, 1, 6);
                return oct;
            }

            float fbm(float2 p, int oct, float lac, float gain, float power, float seed)
            {
                float amp = 1.0;
                float sum = 0.0;
                float norm = 0.0;
                [unroll] for (int i = 0; i < 6; i++)
                {
                    if (i >= oct) break;
                    sum  += vnoise(p, seed + (float)i * 19.19) * amp;
                    norm += amp;
                    p    *= lac;
                    amp  *= gain;
                }
                float h = (norm > 0.0) ? (sum / norm) : 0.0;
                return pow(saturate(h), power);
            }

            // ---------- TBN ----------
            void BuildTBN(float3 normalWS, float4 tangentOS, out float3 T, out float3 B, out float3 N)
            {
                float3 tWS = normalize(TransformObjectToWorldDir(tangentOS.xyz));
                float3 nWS = normalize(normalWS);
                float  w   = tangentOS.w * GetOddNegativeScale();
                float3 bWS = normalize(cross(nWS, tWS) * w);
                T = tWS; B = bWS; N = nWS;
            }

            // ---------- VERTEX ----------
            Varyings vert (Attributes v)
            {
                Varyings o;

                float2 uvN = v.uv * _NoiseUVTiling.xy + _NoiseUVTiling.zw + _Time.y * _NoiseSpeed.xy;
                uvN *= _NoiseScale;

                int   oct  = ClampOctaves(_NoiseOctaves);
                float h    = fbm(uvN, oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);
                float disp = (h - 0.5) * 2.0 * _DisplaceAmp;

                float3 posWS = TransformObjectToWorld(v.positionOS.xyz);
                float3 nWS   = TransformObjectToWorldNormal(v.normalOS);

                posWS += nWS * disp;

                o.positionWS = posWS;
                o.normalWS   = nWS;
                o.positionCS = TransformWorldToHClip(posWS);

                float3 T,B,N;
                BuildTBN(nWS, v.tangentOS, T,B,N);
                o.tangentWS = T;
                o.bitanWS   = B;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float3 cameraWS = _WorldSpaceCameraPos;
                o.viewDirWS = normalize(cameraWS - posWS);

                o.screenPos = ComputeScreenPos(o.positionCS);

                // Sombras de la luz principal
                #if defined(MAIN_LIGHT_SHADOWS)
                    o.shadowCoord = TransformWorldToShadowCoord(posWS);
                #else
                    o.shadowCoord = float4(0,0,0,0);
                #endif

                // Fog factor (URP)
                o.fogFactor = ComputeFogFactor(o.positionCS.z);
                return o;
            }

            // ---------- Lighting helper ----------
            float3 ShadePBR(float3 albedo, float3 nWS, float3 vWS, float metallic, float smoothness, float4 shadowCoord)
            {
                float roughness = 1.0 - smoothness;

                // Luz principal (con/ sin sombras)
                Light mainLight =
                #if defined(MAIN_LIGHT_SHADOWS)
                    GetMainLight(shadowCoord);
                #else
                    GetMainLight();
                #endif

                float3 lDir  = normalize(mainLight.direction);
                float  NdotL = saturate(dot(nWS, -lDir));
                float3 diffuse = albedo * NdotL * mainLight.color;

                // Ambiente (SH)
                float3 sh = SampleSH(nWS);
                diffuse += albedo * sh;

                // Specular muy simple
                float3 h = normalize(-lDir + vWS);
                float  NdotH = saturate(dot(nWS, h));
                float  specPower = lerp(16.0, 256.0, smoothness);
                float3 specCol   = lerp(0.04.xxx, albedo, metallic);
                float  spec      = pow(NdotH, specPower) * NdotL;
                float3 specular  = specCol * spec * mainLight.color;

                return diffuse + specular;
            }

            // ---------- FRAGMENT ----------
            half4 frag (Varyings IN) : SV_Target
            {
                // Base color
                float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;

                // UVs para ruido/foam
                float2 uv = (IN.uv / _MainTex_ST.xy) * _NoiseUVTiling.xy + _NoiseUVTiling.zw
                            + _Time.y * _NoiseSpeed.xy;
                uv *= _NoiseScale;

                int   oct = ClampOctaves(_NoiseOctaves);
                float hC  = fbm(uv, oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);

                // Derivadas en UV para normal TS
                float d     = _NormalDelta;
                float hU    = fbm(uv + float2(d,0), oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);
                float hV    = fbm(uv + float2(0,d), oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);
                float dhdu  = (hU - hC) / d;
                float dhdv  = (hV - hC) / d;
                float3 nTS  = normalize(float3(-dhdu * _NormalStrength, -dhdv * _NormalStrength, 1.0));

                // TBN real -> normal en mundo
                float3x3 TBN = float3x3(normalize(IN.tangentWS), normalize(IN.bitanWS), normalize(IN.normalWS));
                float3 nWS   = normalize(mul(TBN, nTS));

                // Fresnel
                float  fres = pow(1.0 - saturate(dot(nWS, normalize(IN.viewDirWS))), _FresnelPower) * _FresnelStrength;

                // Intersección (depth)
                float2 uvScreen = IN.screenPos.xy / IN.screenPos.w;
                #if UNITY_UV_STARTS_AT_TOP
                    uvScreen.y = 1.0 - uvScreen.y;
                #endif
                float sceneDepth01 = SampleSceneDepth(uvScreen);
                float sceneEye     = LinearEyeDepth(sceneDepth01, _ZBufferParams);
                float thisEye      = IN.screenPos.w;
                float diff         = max(0.0, sceneEye - thisEye);
                float contact      = saturate(1.0 - diff / max(1e-5, _IntersectionThickness));
                contact           *= _IntersectionStrength;

                // Foam de crestas (altura + pendiente + detalle)
                float slope       = saturate(length(float2(dhdu, dhdv)) * _FoamSlopeFactor);
                float foamHeight  = saturate((hC - _FoamThreshold) * _FoamSharpness);
                float2 uvDetail   = ((IN.uv / _MainTex_ST.xy) * _NoiseUVTiling.xy) * (_NoiseScale * _FoamDetailScale)
                                    + _NoiseUVTiling.zw + _Time.y * _FoamDetailSpeed.xy;
                float  fH         = fbm(uvDetail, oct, _NoiseLacunarity, _NoiseGain, 1.0, _Seed + 77.0);
                float  foamDetail = saturate(fH * 1.3 - 0.3);
                float  foamCrests = saturate((foamHeight + slope * 0.7) * _FoamIntensity);
                foamCrests        = saturate(foamCrests * (0.6 + 0.4 * foamDetail));

                float3 rim   = _EdgeColor.rgb * fres;
                float3 foam  = _FoamColor.rgb * foamCrests + _FoamContactColor.rgb * contact;

                // Iluminación
                float3 litColor = ShadePBR(baseCol.rgb, nWS, normalize(IN.viewDirWS), _Metallic, _Glossiness, IN.shadowCoord);

                // Emisión para rim/foam
                float3 emiss = rim + foam * 0.6;

                // Alpha
                float alpha = saturate(_BaseAlpha + fres * 0.4 + (foamCrests + contact) * 0.35);

                // FOG en URP — ¡aquí estaba el error!
                float3 finalRGB = litColor + emiss;
                finalRGB = MixFog(finalRGB, IN.fogFactor);   // <— reemplaza ApplyFog

                return half4(finalRGB, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}