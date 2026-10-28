Shader "Custom/EmaceArt/URP_ArtWater"
{
    Properties
    {
        _WaterColor       ("Color Water",       Color)      = (0.13, 0.68, 0.63, 0.72)
        _DepthColor       ("Color Depth",       Color)      = (0.03, 0.20, 0.45, 1.00)
        _FoamColor        ("Color Foam",        Color)      = (0.95, 0.97, 1.00, 1.00)
        _Depth            ("Depth",             Float)      = 2.0
        _OpacityIntensity ("Opacity Intensity", Range(0,1)) = 0.50
        _SizeFoam         ("Size Foam",         Range(0,1)) = 0.14
        _Gloss            ("Gloss",             Range(0,1)) = 0.11
        _Specular         ("Specular",          Range(0,1)) = 0.65
        _NoiseTex         ("Noise",             2D)         = "white" {}
        _Speed            ("Speed",             Float)      = 0.20
        _NoiseIntensity   ("Noise Intensity",   Range(0,1)) = 0.23
        _ScaleNoise       ("Scale Noise",       Range(-2,2)) = 0.35

        [Space(6)]
        [Header(Shore Glow)]
        _ShoreGlow        ("  Intensity",       Range(0,1))   = 0.18
        _ShoreWidth       ("  Width",           Range(0,1))   = 0.30

        [Space(6)]
        [Header(Style)]
        [Toggle] _CelEnabled  ("  Vectorize (Cel)", Float)        = 0
        _CelSteps             ("  Cel Steps",        Range(1,8))   = 3
        _CelBias              ("  Cel Bias",         Range(0.1,4)) = 1.0

        [Space(6)]
        [Header(Scatter)]
        [Toggle] _ScatterEnabled ("  Enable",         Float)        = 0
        _ScatterScale            ("  Scale",          Range(0.1,4)) = 0.58
        _ScatterDensity          ("  Density",        Range(0,1))   = 0.43
        _ScatterOpacity          ("  Opacity",        Range(0,1))   = 0.69
        _ScatterSpeed            ("  Speed",          Float)        = 0.08

        [Space(6)]
        [Header(Reflections)]
        [Toggle] _ReflEnabled ("  Enable",           Float)        = 0
        _ReflScale            ("  Scale",            Range(0.1,6)) = 2.0
        _ReflDensity          ("  Density",          Range(0,1))   = 0.55
        _ReflBrightness       ("  Brightness",       Range(0,1))   = 0.70
        _ReflSpeed            ("  Speed",            Float)        = 0.15
        _ReflSharpness        ("  Sharpness",        Range(1,32))  = 12.0

        [Space(6)]
        [Header(Caustics)]
        [Toggle] _CausticEnabled ("  Enable",        Float)        = 1
        _CausticStrength      ("  Strength",         Range(0,0.5)) = 0.12
        _CausticScale         ("  Scale",            Range(0.1,6)) = 1.0
        _CausticSharpness     ("  Sharpness",        Range(1,20))  = 8.0

        [Space(6)]
        [Header(Waves)]
        _WaveHeight       ("  Height",          Float)      = 0.05
        _WaveSpeed        ("  Speed",           Float)      = 0.8
        _WaveScale        ("  Scale",           Float)      = 1.0

        [Space(6)]
        [Header(Low Poly)]
        [Toggle] _PolyEnabled  ("  Enable",        Float)        = 1
        _PolyDensity           ("  Tri Size",      Range(1,40))  = 8.0
        _PolyStrength          ("  Height",        Range(0,0.5)) = 0.03
        _PolyNormalAmp         ("  Specular Amp",  Range(1,10))  = 2.0
        _PolySpeed             ("  Speed",         Float)        = 0.4

        [HideInInspector] _Mem_Cel_Steps  ("", Float) = -1
        [HideInInspector] _Mem_Cel_Bias   ("", Float) = -1
        [HideInInspector] _Mem_Grad_Steps ("", Float) = -1
        [HideInInspector] _Mem_Grad_Bias  ("", Float) = -1

        [Space(6)]
        [Header(SWAMP)]
        [Toggle(_SWAMP)]  _SwampOn        ("  Enable Swamp",  Float)      = 0
        _SwampWaterColor  ("  Color Water", Color)      = (0.28, 0.40, 0.14, 0.90)
        _SwampDepthColor  ("  Color Depth", Color)      = (0.10, 0.20, 0.08, 1.00)
        _SwampFoamColor   ("  Color Foam",  Color)      = (0.62, 0.65, 0.42, 1.00)
        _SwampMurkColor   ("  Murk Color",  Color)      = (0.20, 0.28, 0.10, 1.00)
        _SwampMurkiness   ("  Murkiness",   Range(0,1)) = 0.40
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex   vert
            #pragma fragment frag
            #pragma shader_feature_local _SWAMP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4  _WaterColor;
                half4  _DepthColor;
                half4  _FoamColor;
                float  _Depth;
                float  _OpacityIntensity;
                float  _SizeFoam;
                float  _Gloss;
                float  _Specular;
                float4 _NoiseTex_ST;
                float  _Speed;
                float  _NoiseIntensity;
                float  _ScaleNoise;
                float  _ShoreGlow;
                float  _ShoreWidth;
                float  _CelEnabled;
                float  _CelSteps;
                float  _CelBias;
                float  _ScatterEnabled;
                float  _ScatterScale;
                float  _ScatterDensity;
                float  _ScatterOpacity;
                float  _ScatterSpeed;
                float  _ReflEnabled;
                float  _ReflScale;
                float  _ReflDensity;
                float  _ReflBrightness;
                float  _ReflSpeed;
                float  _ReflSharpness;
                float  _CausticEnabled;
                float  _CausticStrength;
                float  _CausticScale;
                float  _CausticSharpness;
                float  _WaveHeight;
                float  _WaveSpeed;
                float  _WaveScale;
                float  _PolyEnabled;
                float  _PolyStrength;
                float  _PolyDensity;
                float  _PolyNormalAmp;
                float  _PolySpeed;
                float  _SwampOn;
                half4  _SwampWaterColor;
                half4  _SwampDepthColor;
                half4  _SwampFoamColor;
                half4  _SwampMurkColor;
                float  _SwampMurkiness;
                float  _Mem_Cel_Steps;
                float  _Mem_Cel_Bias;
                float  _Mem_Grad_Steps;
                float  _Mem_Grad_Bias;
            CBUFFER_END

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos  : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ── Value noise 0..1 ──────────────────────────────────────────────
            float VNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float va = frac(sin(dot(i,               float2(127.1, 311.7))) * 43758.5);
                float vb = frac(sin(dot(i + float2(1,0), float2(127.1, 311.7))) * 43758.5);
                float vc = frac(sin(dot(i + float2(0,1), float2(127.1, 311.7))) * 43758.5);
                float vd = frac(sin(dot(i + float2(1,1), float2(127.1, 311.7))) * 43758.5);
                return lerp(lerp(va, vb, u.x), lerp(vc, vd, u.x), u.y);
            }

            // ── Worley (cellular) noise — returns (f1, f2, cellID) ────────────
            float3 Worley(float2 p)
            {
                float2 pi = floor(p);
                float2 pf = frac(p);
                float wf1  = 8.0;
                float wf2  = 8.0;
                float wcid = 0.0;
                for (int wx = -1; wx <= 1; wx++)
                {
                    for (int wy = -1; wy <= 1; wy++)
                    {
                        float2 g  = float2(wx, wy);
                        float2 o  = float2(
                            frac(sin(dot(pi + g, float2(127.1, 311.7))) * 43758.5),
                            frac(sin(dot(pi + g, float2(269.5, 183.3))) * 12345.6)
                        );
                        float wd = length(g + o - pf);
                        if (wd < wf1)
                        {
                            wf2  = wf1;
                            wf1  = wd;
                            wcid = frac(sin(dot(pi + g, float2(41.3, 289.1))) * 7654.3);
                        }
                        else if (wd < wf2)
                        {
                            wf2 = wd;
                        }
                    }
                }
                return float3(wf1, wf2, wcid);
            }

            // ── Reconstruct world position from screen UV + raw depth ─────────
            float3 ReconstructWorldPos(float2 suv, float rawDepth)
            {
                float4 posCS = float4(suv * 2.0 - 1.0, rawDepth, 1.0);
                #if UNITY_UV_STARTS_AT_TOP
                    posCS.y = -posCS.y;
                #endif
                float4 posWS4 = mul(UNITY_MATRIX_I_VP, posCS);
                return posWS4.xyz / posWS4.w;
            }

            // ── Wave displacement ─────────────────────────────────────────────
            float WaveY(float2 xz, float t)
            {
                float sc = _WaveScale;
                return (  sin(xz.x * sc         + t * _WaveSpeed)       * 0.6
                        + sin(xz.y * sc * 1.3   + t * _WaveSpeed * 0.7) * 0.4
                       ) * _WaveHeight;
            }

            // ── Vertex ────────────────────────────────────────────────────────
            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 pos = IN.positionOS.xyz;
                pos.y += WaveY(pos.xz, _Time.y);

                // Low-poly faceted displacement — hash per grid cell
                if (_PolyEnabled > 0.5)
                {
                    float2 pGrid = floor(pos.xz * _PolyDensity);
                    float  ph    = frac(sin(dot(pGrid, float2(127.1, 311.7))) * 43758.5);
                    pos.y += (ph - 0.5) * _PolyStrength
                           * (0.8 + 0.2 * sin(_Time.y * _PolySpeed + ph * 6.283));
                }

                VertexPositionInputs vi = GetVertexPositionInputs(pos);
                OUT.positionCS = vi.positionCS;
                OUT.screenPos  = ComputeScreenPos(vi.positionCS);
                OUT.positionWS = vi.positionWS;
                return OUT;
            }

            // ── Fragment ──────────────────────────────────────────────────────
            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv       = IN.screenPos.xy / IN.screenPos.w;
                float  waterDep = IN.screenPos.w;
                float  wY       = IN.positionWS.y;
                float2 ts       = 1.0 / _ScreenParams.xy;
                float  sp       = 4.0;

                // ── DUAL-TRACK DEPTH ARCHITECTURE ────────────────────────────
                //
                // TOR A — Ciagly (Smooth):  depthSmooth
                //   foam, shore glow, alpha, fresnel, scatter, caustics, reflections
                //   NIGDY nie przechodzi przez floor() / kwantyzacje
                //
                // TOR B — Dyskretny (Quantized): depthQ
                //   TYLKO kolor wody (lerp WaterColor → DepthColor)
                //
                // Center tap — full pipeline z Y-safety check (1x matrix multiply)
                float  rawCenter    = SampleSceneDepth(uv);
                float  sceneZctr    = LinearEyeDepth(rawCenter, _ZBufferParams);
                float3 sceneWP      = ReconstructWorldPos(uv, rawCenter);
                float  d0           = (sceneWP.y > wY + 0.08)
                                    ? 99999.0
                                    : max(0.0, sceneZctr - waterDep);  // metry

                // 4 offset taps — lightweight (bez WorldPos), ale z tym samym zabezpieczeniem:
                // jezeli obiekt jest POWYZEJ wody (ez < waterDep), zwracamy 99999 — nie 0.
                // max(0, ujemna_roznica) = 0 powodowaloby minDiff=0 → foam na calej scenie.
                float ez1 = LinearEyeDepth(SampleSceneDepth(uv + ts * float2( sp, 0.0)), _ZBufferParams);
                float ez2 = LinearEyeDepth(SampleSceneDepth(uv + ts * float2(-sp, 0.0)), _ZBufferParams);
                float ez3 = LinearEyeDepth(SampleSceneDepth(uv + ts * float2(0.0,  sp)), _ZBufferParams);
                float ez4 = LinearEyeDepth(SampleSceneDepth(uv + ts * float2(0.0, -sp)), _ZBufferParams);
                float d1 = (ez1 < waterDep) ? 99999.0 : (ez1 - waterDep);
                float d2 = (ez2 < waterDep) ? 99999.0 : (ez2 - waterDep);
                float d3 = (ez3 < waterDep) ? 99999.0 : (ez3 - waterDep);
                float d4 = (ez4 < waterDep) ? 99999.0 : (ez4 - waterDep);
                float  minDiff = min(d0, min(min(d1, d2), min(d3, d4)));  // metry

                // TOR A — ciagly [0,1]
                float depthSmooth = saturate(minDiff / max(0.001, _Depth));

                // TOR B — dyskretny [0,1]
                float depthQ = depthSmooth;
                if (_CelEnabled > 0.5)
                {
                    float depthB = pow(depthSmooth, max(0.01, _CelBias));
                    depthQ = floor(depthB * max(1.0, _CelSteps)) / max(1.0, _CelSteps);
                }

                // ── Geometric normal + Low Poly specular prep ─────────────────
                // ddx/ddy musza byc w uniform control flow (poza branchami)
                float3 geoNormal = normalize(cross(ddx(IN.positionWS), ddy(IN.positionWS)));
                geoNormal = (geoNormal.y < 0.0) ? -geoNormal : geoNormal;

                // PolyNormalAmp efektywne tylko gdy jest realne przemieszczenie werteksow.
                // Guard: hasDisplacement → 0 gdy PolyStrength≈0, 1 gdy >0.025
                float polyDepthFade   = saturate((depthSmooth - 0.15) * 5.0);
                float hasDisplacement = saturate(_PolyStrength * 40.0);
                float polyAmp         = lerp(1.0, lerp(1.0, _PolyNormalAmp, hasDisplacement), polyDepthFade);
                float3 tiltXZ  = float3(geoNormal.x, 0.0, geoNormal.z) * polyAmp;
                float3 surfNormal = (_PolyEnabled > 0.5)
                                  ? normalize(float3(tiltXZ.x, geoNormal.y, tiltXZ.z))
                                  : float3(0.0, 1.0, 0.0);

                // ── Swamp modifiers ───────────────────────────────────────────
                float fresnelScale = 1.0;
                float causticScale = 1.0;
                #if defined(_SWAMP)
                    depthQ       = lerp(depthQ, depthSmooth, _SwampMurkiness * 0.75);
                    causticScale = saturate(1.0 - _SwampMurkiness * 0.90);
                    fresnelScale = 1.0 + _SwampMurkiness * 0.65;
                #endif

                // ── Colour set ────────────────────────────────────────────────
                #if defined(_SWAMP)
                    half4 cWater = _SwampWaterColor;
                    half4 cDepth = _SwampDepthColor;
                    half4 cFoam  = _SwampFoamColor;
                #else
                    half4 cWater = _WaterColor;
                    half4 cDepth = _DepthColor;
                    half4 cFoam  = _FoamColor;
                #endif

                // ── Base water colour (TOR B) ─────────────────────────────────
                half4 col = lerp(cWater, cDepth, depthQ);
                col.a     = lerp(cWater.a, 1.0, depthQ * _OpacityIntensity);

                // ── Fresnel (TOR A) ───────────────────────────────────────────
                float3 viewDir = normalize(GetWorldSpaceViewDir(IN.positionWS));
                float  fresnel = pow(1.0 - saturate(dot(surfNormal, viewDir)), 3.0);
                col.a   = saturate(col.a + fresnel * 0.35 * fresnelScale);
                col.rgb = lerp(col.rgb, cDepth.rgb, fresnel * 0.22 * fresnelScale * (1.0 - depthQ));

                // ── Swamp murk (TOR A) ────────────────────────────────────────
                #if defined(_SWAMP)
                    float murkFade = _SwampMurkiness * (1.0 - depthSmooth * 0.55);
                    col.rgb = lerp(col.rgb, _SwampMurkColor.rgb, murkFade);
                    col.a   = saturate(col.a + _SwampMurkiness * 0.25);
                #endif

                // ── Noise UV — ScaleNoise ujemne = odwrocony kierunek animacji
                float  absScale  = max(0.001, abs(_ScaleNoise));
                float  noiseSign = (_ScaleNoise < 0.0) ? -1.0 : 1.0;
                float2 noiseBase = IN.positionWS.xz * absScale * noiseSign
                                 + _Time.y * _Speed * float2(1.0, 0.6);
                float  nA    = VNoise(noiseBase * 12.0);
                float  nB    = VNoise(noiseBase *  6.0 + float2(3.7, 1.4));
                float  nTex  = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseBase * 0.45).r;
                float  noise = (nA * 0.60 + nB * 0.40) * nTex;

                // ── Foam mask — fwidth na surowej rozlicy w metrach (TOR A) ───
                // foamEdge znormalizowane [0,1] przeliczamy na metry dla spojnosci z minDiff
                float foamEdge  = _SizeFoam + noise * _NoiseIntensity;        // [0,1]
                float foamEdgeM = foamEdge * max(0.001, _Depth);              // metry
                float edgeWidth = fwidth(d0) * 1.5;                           // metry (center tap)
                float foamMask  = 1.0 - smoothstep(foamEdgeM, foamEdgeM + edgeWidth, minDiff);
                col.rgb = lerp(col.rgb, cFoam.rgb, foamMask * cFoam.a);
                col.a   = max(col.a, foamMask * 0.96);

                // ── Shore Glow — Bokashi halo (TOR A) ────────────────────────
                // Smoothstep od krawedzi piany w strone glebokiej wody.
                // Porownanie w przestrzeni znormalizowanej [0,1].
                float shoreOffset   = max(0.0, depthSmooth - foamEdge);
                float shoreGlowMask = (1.0 - smoothstep(0.0, max(0.001, _ShoreWidth), shoreOffset))
                                    * (1.0 - foamMask);
                col.rgb = lerp(col.rgb, cFoam.rgb, shoreGlowMask * _ShoreGlow);
                col.a   = saturate(col.a + shoreGlowMask * _ShoreGlow * 0.2);

                // ── Scatter — podwodne rozpraszanie swiatla (TOR A) ───────────
                // Miekkie, pulsujace przeswity w plytkich strefach.
                // Worley komórki tworza organiczne plamy swiatla.
                if (_ScatterEnabled > 0.5)
                {
                    float2 sUV    = IN.positionWS.xz * _ScatterScale
                                  + _Time.y * _ScatterSpeed * float2(0.7, 0.4);
                    float3 sCell  = Worley(sUV * 2.5);
                    float  sDist  = sCell.x;
                    float  sCID   = sCell.z;
                    float  sfw    = fwidth(sDist) * 2.0;
                    float  sRadius = _ScatterDensity * 0.5;
                    float  sMask  = 1.0 - smoothstep(sRadius - sfw, sRadius + sfw, sDist);
                    float  sPulse = 0.7 + 0.3 * sin(_Time.y * _ScatterSpeed * 3.0 + sCID * 9.4);
                    float  sZone  = (1.0 - depthSmooth) * (1.0 - foamMask);
                    float  sGlow  = sMask * sPulse * sZone * _ScatterOpacity;
                    col.rgb = lerp(col.rgb, cWater.rgb * 1.6h, sGlow);
                    col.a   = saturate(col.a + sGlow * 0.1);
                }

                // ── Caustics — trojkatowa interferencja sinusow (TOR A) ────────
                if (_CausticEnabled > 0.5)
                {
                    float2 cp = IN.positionWS.xz;
                    float  ck = absScale * _CausticScale * 6.0;  // absScale — bez znaku
                    float  ct = _Time.y * _Speed * 0.4;

                    float cA = saturate(sin(cp.x * ck + ct * 0.60) * sin(cp.y * ck + ct * 0.45) * 0.5 + 0.5);
                    float cB = saturate(sin((cp.x * 0.866 + cp.y * 0.500) * ck + ct * 0.50)
                                      * sin((cp.x * 0.500 - cp.y * 0.866) * ck + ct * 0.38) * 0.5 + 0.5);
                    float cC = saturate(sin((cp.x * 0.500 + cp.y * 0.866) * ck + ct * 0.42)
                                      * sin((cp.x * 0.866 - cp.y * 0.500) * ck + ct * 0.55) * 0.5 + 0.5);

                    float caustic  = pow(cA * cB * cC, _CausticSharpness * 0.35);
                    float cShimmer = 0.6 + 0.4 * sin(ct * 6.0 + cp.x * 1.3 + cp.y * 0.9);
                    caustic        = saturate(caustic * cShimmer * 2.5);
                    float cZone    = (1.0 - depthSmooth) * (1.0 - foamMask);  // TOR A
                    col.rgb       += caustic * cZone * (_CausticStrength * causticScale);
                }

                // ── Reflections — Worley glint spots ─────────────────────────
                // fwidth obliczany poza branchem (uniform control flow)
                float2 rBase   = IN.positionWS.xz * _ReflScale;
                float  rT      = _Time.y * _ReflSpeed;
                float2 rUV     = rBase + float2(rT * 0.7, rT * 0.5);
                float3 wcell   = Worley(rUV);
                float  rf1     = wcell.x;
                float  rCellID = wcell.z;
                float  rWfw    = fwidth(rf1) * 1.5;

                if (_ReflEnabled > 0.5)
                {
                    float rRadius  = _ReflDensity * 0.45;
                    float discMask = 1.0 - smoothstep(rRadius - rWfw, rRadius + rWfw, rf1);

                    float rLifePhase = rCellID * 6.283;
                    float rLifeRate  = 0.9 + rCellID * 1.2;
                    float rLife      = smoothstep(0.2, 0.8,
                                        0.5 + 0.5 * sin(rT * rLifeRate + rLifePhase));
                    float rFlash     = pow(0.5 + 0.5 * sin(rT * 7.0 + rCellID * 17.3), _ReflSharpness);
                    float rGlint     = discMask * rLife * (0.3 + 0.7 * rFlash);
                    rGlint          *= saturate(1.0 - depthSmooth * 0.5) * (1.0 - foamMask * 0.95);
                    col.rgb         += rGlint * _ReflBrightness;
                }

                // ── Specular highlight — Blinn-Phong ──────────────────────────
                Light  mainLight = GetMainLight();
                float3 halfDir   = normalize(mainLight.direction + viewDir);
                float  ndoth     = max(0.0, dot(surfNormal, halfDir));
                float  spec      = pow(ndoth, max(1.0, _Gloss * 256.0))
                                 * _Specular * (1.0 - foamMask);
                col.rgb += mainLight.color * spec;

                return col;
            }
            ENDHLSL
        }
    }

    CustomEditor "ArtWaterEditor"
    FallBack "Hidden/InternalErrorShader"
}
