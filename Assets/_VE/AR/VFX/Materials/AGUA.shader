Shader "Custom/AGUA_ProcNoise_Foam_UV"
{
    Properties
    {
        _Color ("Color base", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        _Glossiness ("Smoothness", Range(0,1)) = 0.85
        _Metallic   ("Metallic",   Range(0,1)) = 0.0

        // -------- Procedural Noise (FBM) en UV --------
        _NoiseScale      ("Noise Scale (UV)", Float) = 1.0
        _NoiseOctaves    ("Noise Octaves (1-6)", Range(1,6)) = 4
        _NoiseLacunarity ("Lacunarity", Float) = 2.0
        _NoiseGain       ("Gain", Float) = 0.5
        _NoisePower      ("Noise Power", Range(0.5,4.0)) = 1.4
        _NoiseSpeed      ("Noise Speed (UV XY)", Vector) = (0.2, 0.1, 0, 0)
        _Seed            ("Seed", Float) = 0.0
        _NoiseUVTiling   ("Noise UV Tiling (xy), Offset (zw)", Vector) = (1,1,0,0)

        // -------- Desplazamiento / Normales --------
        _DisplaceAmp     ("Amplitud desplazamiento", Range(0,0.5)) = 0.05
        _NormalStrength  ("Fuerza normal", Range(0,4)) = 1.5
        _NormalDelta     ("Delta derivadas", Range(0.001, 0.2)) = 0.02

        // -------- Fresnel --------
        _FresnelPower    ("Fresnel Power", Range(0.5,8.0)) = 3.0
        _FresnelStrength ("Fresnel Strength", Range(0,2)) = 0.8
        _EdgeColor       ("Fresnel/Edge Color", Color) = (0.75, 0.9, 1.0, 1.0)
        _BaseAlpha       ("Alpha base", Range(0,1)) = 0.5

        // -------- Intersección (Depth) --------
        _IntersectionThickness ("Grosor intersección", Range(0.001, 1.0)) = 0.2
        _IntersectionStrength  ("Fuerza intersección", Range(0,3)) = 1.2
        _FoamContactColor      ("Color espuma contacto", Color) = (1,1,1,1)

        // -------- Foam de crestas --------
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
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 350

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows vertex:vert
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;

        // Depth (declaración directa para máxima compatibilidad)
        sampler2D _CameraDepthTexture;

        // Noise params
        float  _NoiseScale, _NoiseLacunarity, _NoiseGain, _NoisePower;
        float4 _NoiseSpeed; // xy
        float  _Seed, _NoiseOctaves;
        float4 _NoiseUVTiling; // xy tiling, zw offset

        // Displace/Normals
        float  _DisplaceAmp, _NormalStrength, _NormalDelta;

        // Fresnel
        float  _FresnelPower, _FresnelStrength, _BaseAlpha;
        fixed4 _EdgeColor;

        // Intersección
        float  _IntersectionThickness, _IntersectionStrength;
        fixed4 _FoamContactColor;

        // Foam crestas
        fixed4 _FoamColor;
        float  _FoamThreshold, _FoamSlopeFactor, _FoamSharpness, _FoamIntensity;
        float  _FoamDetailScale;
        float4 _FoamDetailSpeed; // xy

        struct Input
        {
            float2 uv_MainTex;  // usaremos UV0 como coordenada base
            float3 viewDir;
            float4 screenPos;
        };

        // -------- Value noise + FBM --------
        float rand2d(float2 p, float seed)
        {
            return frac(sin(dot(p, float2(12.9898,78.233)) + seed) * 43758.5453123);
        }

        float noise2(float2 p, float seed)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            float2 u = f*f*f*(f*(f*6 - 15) + 10);

            float a = rand2d(i + float2(0,0), seed);
            float b = rand2d(i + float2(1,0), seed);
            float c = rand2d(i + float2(0,1), seed);
            float d = rand2d(i + float2(1,1), seed);

            float ab = lerp(a, b, u.x);
            float cd = lerp(c, d, u.x);
            return lerp(ab, cd, u.y);
        }

        int ClampOctaves(float v)
        {
            int oct = (int)floor(v + 0.5);
            if (oct < 1) oct = 1; if (oct > 6) oct = 6;
            return oct;
        }

        float fbm(float2 p, int oct, float lac, float gain, float power, float seed)
        {
            float amp = 1.0, sum = 0.0, norm = 0.0;
            [unroll] for (int i = 0; i < 6; i++)
            {
                if (i >= oct) break;
                sum  += noise2(p, seed + (float)i * 19.19) * amp;
                norm += amp;
                p    *= lac;
                amp  *= gain;
            }
            float h = (norm > 0.0) ? (sum / norm) : 0.0;
            return pow(saturate(h), power);
        }

        float SampleSceneEye(float4 sp)
        {
            float raw = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(sp));
            return LinearEyeDepth(raw);
        }

        // ---------- Vertex: displacement (UV) ----------
        void vert (inout appdata_full v)
        {
            float2 uv = v.texcoord.xy * _NoiseUVTiling.xy + _NoiseUVTiling.zw + _Time.y * _NoiseSpeed.xy;
            uv *= _NoiseScale;

            int oct = ClampOctaves(_NoiseOctaves);
            float h = fbm(uv, oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);

            float disp = (h - 0.5) * 2.0 * _DisplaceAmp;

            float3 wpos  = mul(unity_ObjectToWorld, v.vertex).xyz;
            float3 wnorm = UnityObjectToWorldNormal(v.normal);
            wpos += wnorm * disp;

            v.vertex = mul(unity_WorldToObject, float4(wpos,1));
        }

        // ---------- Surface (UV) ----------
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseCol = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // UV para el ruido
            float2 uv = IN.uv_MainTex * _NoiseUVTiling.xy + _NoiseUVTiling.zw + _Time.y * _NoiseSpeed.xy;
            uv *= _NoiseScale;

            int oct = ClampOctaves(_NoiseOctaves);

            float hC = fbm(uv, oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);

            // Normales desde derivadas en UV (no mundo)
            float d = _NormalDelta;
            float hU = fbm(uv + float2(d, 0), oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);
            float hV = fbm(uv + float2(0, d), oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);
            float dhdu = (hU - hC) / d;
            float dhdv = (hV - hC) / d;

            // Tangent space sintético donde Z es “normal base” del mapa
            float3 nT = normalize(float3(-dhdu * _NormalStrength, -dhdv * _NormalStrength, 1.0));
            o.Normal = nT;

            // FRESNEL (en este espacio tangente)
            float3 N = normalize(float3(0,0,1));
            float3 V = normalize(IN.viewDir);
            float  fres  = pow(1.0 - saturate(dot(N, V)), _FresnelPower) * _FresnelStrength;

            // INTERSECCIÓN (espuma de contacto)
            float sceneEye = SampleSceneEye(IN.screenPos);
            float thisEye  = IN.screenPos.w;
            float diff = max(0.0, sceneEye - thisEye);
            float contact = saturate(1.0 - diff / max(1e-5, _IntersectionThickness));
            contact *= _IntersectionStrength;

            // FOAM en crestas (altura + pendiente + detalle)
            float slope = saturate(length(float2(dhdu, dhdv)) * _FoamSlopeFactor);
            float foamHeight = saturate((hC - _FoamThreshold) * _FoamSharpness);

            float2 uvDetail = (IN.uv_MainTex * _NoiseUVTiling.xy) * (_NoiseScale * _FoamDetailScale)
                              + _NoiseUVTiling.zw + _Time.y * _FoamDetailSpeed.xy;
            float  fH = fbm(uvDetail, oct, _NoiseLacunarity, _NoiseGain, 1.0, _Seed + 77.0);
            float  foamDetail = saturate(fH * 1.3 - 0.3);

            float foamCrests = saturate((foamHeight + slope * 0.7) * _FoamIntensity);
            foamCrests = saturate(foamCrests * (0.6 + 0.4 * foamDetail));

            // Composición
            float3 rim  = _EdgeColor.rgb * fres;
            float3 foam = _FoamColor.rgb * foamCrests + _FoamContactColor.rgb * contact;

            o.Albedo     = baseCol.rgb;
            o.Metallic   = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission   = rim + foam * 0.6;
            o.Alpha      = saturate(_BaseAlpha + fres * 0.4 + (foamCrests + contact) * 0.35);
        }
        ENDCG
    }

    FallBack "Transparent"
}