Shader "Custom/URP_LowPolyWater"
{
    Properties
    {
        [Header(Colors)]
        _ShallowColor     ("  Shallow Color",      Color)      = (0.11, 0.91, 0.71, 0.6)
        _DeepColor        ("  Deep Color",         Color)      = (0.05, 0.28, 0.63, 1.0)
        _FoamColor        ("  Foam Color",         Color)      = (0.91, 0.96, 1.00, 1.0)

        [Space(10)]
        [Header(Waves)]
        _DepthFade        ("  Depth Fade",         Float)      = 2.0
        _WaveHeight       ("  Wave Height",        Float)      = 0.15
        _WaveSpeed        ("  Wave Speed",         Float)      = 1.0
        _WaveScale        ("  Wave Scale",         Float)      = 1.0

        [Space(10)]
        [Header(Foam)]
        _FoamDepth        ("  Width",              Float)      = 0.30
        _FoamSharpness    ("  Sharpness",          Range(0,1)) = 0.70
        _FoamVariation    ("  Variation",          Range(0,1)) = 0.50
        _FoamFlowX        ("  Flow Direction X",   Float)      = 0.30
        _FoamFlowZ        ("  Flow Direction Z",   Float)      = 0.00
        _FoamDistortScale ("  Distort Scale",      Float)      = 1.8
        _FoamDistortStr   ("  Distort Strength",   Float)      = 0.35
        _FoamDistortSpeed ("  Distort Speed",      Float)      = 0.35

        [Space(10)]
        [Header(Underwater Distortion)]
        _UnderwaterDistortStr   ("  Strength",     Range(0,0.12)) = 0.04
        _UnderwaterDistortScale ("  Scale",        Float)         = 4.0
        _UnderwaterDistortSpeed ("  Speed",        Float)         = 0.5
        _UnderwaterVisibility   ("  Visibility",   Range(0,1))    = 0.55

        [Space(10)]
        [Header(Caustics)]
        _CausticsColor    ("  Color",              Color)      = (0.6, 1.0, 0.85, 1.0)
        _CausticsStr      ("  Strength",           Range(0,1)) = 0.30
        _CausticsScale    ("  Scale",              Float)      = 3.5
        _CausticsSpeed    ("  Speed",              Float)      = 0.6

        [Space(10)]
        [Header(Dark Patches)]
        _DarkColor        ("  Color",              Color)      = (0.02, 0.10, 0.22, 1.0)
        _DarkPatchStr     ("  Strength",           Range(0,1)) = 0.18
        _DarkScale        ("  Scale",              Float)      = 1.8
        _DarkSpeed        ("  Speed",              Float)      = 0.15

        [Space(10)]
        [Header(Ripples)]
        _RippleStr        ("  Ripple Strength",    Range(0,1)) = 0.08

        [Space(10)]
        [Header(Texture Layer)]
        _BlendTex         ("  Texture",            2D)         = "white" {}
        _BlendColor       ("  Tint",               Color)      = (0.15, 0.35, 0.12, 1.0)
        _BlendStr         ("  Strength",           Range(0,1)) = 0.45
        _BlendScale       ("  Scale",              Float)      = 1.0
        _BlendScrollX     ("  Scroll X",           Float)      = 0.02
        _BlendScrollY     ("  Scroll Y",           Float)      = 0.01
        [KeywordEnum(Multiply, Tint, Overlay)] _BlendMode ("  Blend Mode", Float) = 0

        [Space(10)]
        [Header(SWAMP)]
        [Toggle(_SWAMP_ON)] _SwampOn ("  Enable Swamp Profile", Float) = 0

        _SwampShallowColor  ("  Shallow Color",      Color)      = (0.21, 0.30, 0.11, 0.85)
        _SwampDeepColor     ("  Deep Color",         Color)      = (0.07, 0.11, 0.05, 1.00)
        _SwampFoamColor     ("  Foam Color",         Color)      = (0.62, 0.65, 0.40, 1.00)
        _SwampCausticsColor ("  Caustics Color",     Color)      = (0.35, 0.50, 0.14, 1.00)
        _SwampDarkColor     ("  Dark Patches Color", Color)      = (0.04, 0.06, 0.02, 1.00)
        _SwampTurbidity     ("  Turbidity",          Range(0,1)) = 0.72
        _SwampTurbidColor   ("  Turbid Color",       Color)      = (0.16, 0.20, 0.09, 1.0)
        _SwampOpacity       ("  Opacity",            Range(0,1)) = 0.65

        [Toggle(_SWAMP_SCUM)]    _SwampScumOn    ("  Scum",           Float)      = 1
        _SwampScumColor                          ("  Scum Color",     Color)      = (0.20, 0.25, 0.08, 1.0)
        _SwampScumStr                            ("  Scum Strength",  Range(0,1)) = 0.55

        [Toggle(_SWAMP_ALGAE)]   _SwampAlgaeOn   ("  Algae",          Float)      = 1
        _SwampAlgaeColor                         ("  Algae Color",    Color)      = (0.11, 0.28, 0.05, 1.0)
        _SwampAlgaeStr                           ("  Algae Strength", Range(0,1)) = 0.45

        [Toggle(_SWAMP_BUBBLES)] _SwampBubblesOn ("  Bubbles",         Float)      = 1
        _SwampBubbleColor                        ("  Bubble Color",    Color)      = (0.48, 0.46, 0.26, 1.0)
        _SwampBubbleStr                          ("  Bubble Strength", Range(0,1)) = 0.30

        [Toggle(_SWAMP_SILT)]    _SwampSiltOn    ("  Silt",           Float)      = 1
        _SwampSiltColor                          ("  Silt Color",     Color)      = (0.22, 0.16, 0.07, 1.0)
        _SwampSiltStr                            ("  Silt Strength",  Range(0,1)) = 0.50
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _BLENDMODE_MULTIPLY _BLENDMODE_TINT _BLENDMODE_OVERLAY
            #pragma shader_feature _SWAMP_ON
            #pragma shader_feature _SWAMP_SCUM
            #pragma shader_feature _SWAMP_ALGAE
            #pragma shader_feature _SWAMP_BUBBLES
            #pragma shader_feature _SWAMP_SILT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4  _ShallowColor;
                half4  _DeepColor;
                half4  _FoamColor;
                float  _DepthFade;
                float  _WaveHeight;
                float  _WaveSpeed;
                float  _WaveScale;
                float  _FoamDepth;
                float  _FoamSharpness;
                float  _FoamVariation;
                float  _FoamFlowX;
                float  _FoamFlowZ;
                float  _FoamDistortScale;
                float  _FoamDistortStr;
                float  _FoamDistortSpeed;
                float  _UnderwaterDistortStr;
                float  _UnderwaterDistortScale;
                float  _UnderwaterDistortSpeed;
                float  _UnderwaterVisibility;
                half4  _CausticsColor;
                float  _CausticsStr;
                float  _CausticsScale;
                float  _CausticsSpeed;
                half4  _DarkColor;
                float  _DarkPatchStr;
                float  _DarkScale;
                float  _DarkSpeed;
                float  _RippleStr;
                half4  _BlendColor;
                float  _BlendStr;
                float  _BlendScale;
                float  _BlendScrollX;
                float  _BlendScrollY;
                float4 _BlendTex_ST;
                half4  _SwampShallowColor;
                half4  _SwampDeepColor;
                half4  _SwampFoamColor;
                half4  _SwampCausticsColor;
                half4  _SwampDarkColor;
                float  _SwampTurbidity;
                half4  _SwampTurbidColor;
                float  _SwampOpacity;
                half4  _SwampScumColor;
                float  _SwampScumStr;
                half4  _SwampAlgaeColor;
                float  _SwampAlgaeStr;
                half4  _SwampBubbleColor;
                float  _SwampBubbleStr;
                half4  _SwampSiltColor;
                float  _SwampSiltStr;
            CBUFFER_END

            TEXTURE2D(_BlendTex);
            SAMPLER(sampler_BlendTex);

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

            // ── Value noise ───────────────────────────────────────────────
            float VNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = frac(sin(dot(i,               float2(127.1, 311.7))) * 43758.5);
                float b = frac(sin(dot(i + float2(1,0), float2(127.1, 311.7))) * 43758.5);
                float c = frac(sin(dot(i + float2(0,1), float2(127.1, 311.7))) * 43758.5);
                float d = frac(sin(dot(i + float2(1,1), float2(127.1, 311.7))) * 43758.5);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // ── Foam FBM — 5 octaves ──────────────────────────────────────
            float FoamFBM(float2 xz, float time)
            {
                float val = 0.0, amp = 0.500, frq = 7.0, spd = 0.30;
                val += VNoise(xz * frq + float2( time*spd*0.70,  time*spd*0.40)                    ) * amp; amp *= 0.52; frq *= 2.0;
                val += VNoise(xz * frq - float2( time*spd*0.50,  time*spd*0.60) + float2(3.7, 1.3)) * amp; amp *= 0.52; frq *= 2.0;
                val += VNoise(xz * frq + float2( time*spd*1.10, -time*spd*0.80) + float2(7.1, 5.4)) * amp; amp *= 0.52; frq *= 2.0;
                val += VNoise(xz * frq - float2( time*spd*0.90,  time*spd*1.30) + float2(2.9, 8.6)) * amp; amp *= 0.52; frq *= 2.0;
                val += VNoise(xz * frq + float2( time*spd*0.60, -time*spd*1.10) + float2(5.3, 3.8)) * amp;
                return saturate(val / 1.001);
            }

            // ── Foam UV distortion ────────────────────────────────────────
            float2 FoamDistortUV(float2 xz, float time)
            {
                float  t  = time * _FoamDistortSpeed;
                float2 s  = xz   * _FoamDistortScale;
                float  dx = VNoise(s + float2( t*.8,  t*.3))                        - VNoise(s + float2(t*.4,  t*.7) + float2(5.2, 1.7));
                float  dy = VNoise(s + float2(-t*.6,  t*.5) + float2(2.3, 8.1)) - VNoise(s + float2(t*.3, -t*.4) + float2(6.7, 3.4));
                return float2(dx, dy) * _FoamDistortStr;
            }

            // ── Underwater refraction offset ──────────────────────────────
            float2 UnderwaterOffset(float2 xz, float time)
            {
                float  t  = time * _UnderwaterDistortSpeed;
                float2 s  = xz   * abs(_UnderwaterDistortScale);
                float  ox = VNoise(s + float2( t*.6,  t*.4))                        - VNoise(s + float2(-t*.5,  t*.7) + float2(4.1, 2.3));
                float  oy = VNoise(s + float2( t*.3, -t*.6) + float2(1.7, 6.8)) - VNoise(s + float2(-t*.4,  t*.3) + float2(8.2, 3.5));
                return float2(ox, oy) * _UnderwaterDistortStr;
            }

            // ── Caustics ──────────────────────────────────────────────────
            float Caustics(float2 xz, float time)
            {
                float2 s  = xz * _CausticsScale;
                float  t  = time * _CausticsSpeed;
                float  n1 = VNoise(s + float2( t*.7,  t*.4));
                float  n2 = VNoise(s + float2(-t*.5,  t*.6) + float2(3.7, 1.3));
                float  n3 = VNoise(s * 1.8 + float2(t*.3, -t*.5) + float2(7.1, 2.9));
                return pow(saturate(1.0 - min(min(n1, n2), n3)), 4.0);
            }

            // ── Dark patches — smooth average, no swirling blobs ──────────
            // pow(avg, 3.5): only very coherent areas darken, no isolated circles
            float DarkPatches(float2 xz, float time)
            {
                float2 s  = xz * abs(_DarkScale);
                float  t  = time * _DarkSpeed;
                float  n1 = VNoise(s + float2( t*.5,  t*.3));
                float  n2 = VNoise(s + float2(-t*.4,  t*.5) + float2(4.3, 2.1));
                float  n3 = VNoise(s * 1.5 + float2(t*.2, -t*.4) + float2(8.7, 5.2));
                return pow(saturate((n1 + n2 + n3) / 3.0), 3.5);
            }

            // ── Ripples — returns 0..1 noise for blend modulation ─────────
            float Ripples(float2 xz, float time)
            {
                float2 s = xz * 6.0;
                float  t = time * 1.2;
                return (  VNoise(s       + float2( t,    t*.7))
                        + VNoise(s * 1.4 + float2(-t*.6, t*.9))
                        + VNoise(s * 0.7 + float2( t*.4,-t*.5))) / 3.0;
            }

            // ── Wave displacement ─────────────────────────────────────────
            float WaveY(float2 xz, float time)
            {
                float sc = _WaveScale;
                return (  sin( xz.x       * sc       + time * _WaveSpeed      ) * 0.5
                        + sin( xz.y       * sc * 1.3 + time * _WaveSpeed * 0.7) * 0.5
                        + sin((xz.x+xz.y) * sc * 0.8 + time * _WaveSpeed * 1.2) * 0.3
                       ) * _WaveHeight;
            }

            // ── Overlay blend ─────────────────────────────────────────────
            half3 BlendOverlay(half3 b, half3 l)
            {
                half3 dark  = 2.0 * b * l;
                half3 light = 1.0 - 2.0 * (1.0 - b) * (1.0 - l);
                return lerp(dark, light, step(0.5, b));
            }

            // ── Swamp helpers ─────────────────────────────────────────────
            float SwampScum(float2 xz, float time)
            {
                float2 s  = xz * 1.8;
                float  t  = time * 0.05;
                float  n1 = VNoise(s       + float2( t*.4,  t*.3));
                float  n2 = VNoise(s * 1.3 + float2(-t*.3,  t*.5) + float2(3.1, 1.7));
                float  n3 = VNoise(s * 0.7 + float2( t*.2, -t*.4) + float2(6.3, 4.1));
                return smoothstep(0.42, 0.72, pow(saturate(max(max(n1, n2), n3)), 2.5));
            }

            float SwampAlgae(float2 xz, float time)
            {
                float val = 0.0, amp = 0.5, frq = 3.5;
                float t = time * 0.08;
                val += VNoise(xz * frq + float2( t*.6,  t*.4)) * amp; amp *= 0.5; frq *= 2.1;
                val += VNoise(xz * frq - float2( t*.5,  t*.3) + float2(2.3, 5.1)) * amp; amp *= 0.5; frq *= 2.1;
                val += VNoise(xz * frq + float2( t*.3, -t*.6) + float2(7.4, 2.8)) * amp;
                return smoothstep(0.38, 0.62, val / 0.875);
            }

            float SwampBubbles(float2 xz, float time)
            {
                float2 s  = xz * 8.0;
                float  t  = time * 1.2;
                float  n1 = VNoise(s       + float2( t*.9,  t*.5));
                float  n2 = VNoise(s       + float2(-t*.7,  t*.8) + float2(1.9, 3.3));
                float  n3 = VNoise(s * 1.4 + float2( t*.4, -t*.6) + float2(5.7, 1.2));
                return smoothstep(0.72, 0.88, 1.0 - pow(saturate(min(min(n1, n2), n3)), 3.0));
            }

            float SwampSilt(float2 xz, float time)
            {
                float2 s  = xz * 2.0;
                float  t  = time * 0.03;
                float  n1 = VNoise(s       + float2( t*.3,  t*.2));
                float  n2 = VNoise(s * 1.5 + float2(-t*.2,  t*.4) + float2(4.4, 2.0));
                return smoothstep(0.30, 0.65, (n1 + n2 * 0.5) / 1.5);
            }

            // ── Vertex ────────────────────────────────────────────────────
            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float3 pos = IN.positionOS.xyz;
                pos.y += WaveY(pos.xz, _Time.y);
                VertexPositionInputs vi = GetVertexPositionInputs(pos);
                OUT.positionCS = vi.positionCS;
                OUT.screenPos  = ComputeScreenPos(vi.positionCS);
                OUT.positionWS = vi.positionWS;
                return OUT;
            }

            // ── Fragment ──────────────────────────────────────────────────
            half4 frag(Varyings IN) : SV_Target
            {
                // ── Depth ─────────────────────────────────────────────────
                float2 baseUV = IN.screenPos.xy / IN.screenPos.w;
                float  raw    = SampleSceneDepth(baseUV);
                float  scene  = LinearEyeDepth(raw, _ZBufferParams);
                float  water  = IN.screenPos.w;
                float  diff   = scene - water;

                float fade        = saturate(diff / max(0.001, _DepthFade));
                float shallowMask = 1.0 - fade;

                // ── Active colour set ─────────────────────────────────────
                #if defined(_SWAMP_ON)
                    half4 wShallow  = _SwampShallowColor;
                    half4 wDeep     = _SwampDeepColor;
                    half4 wFoam     = _SwampFoamColor;
                    half3 wCaustics = _SwampCausticsColor.rgb;
                    half3 wDark     = _SwampDarkColor.rgb;
                #else
                    half4 wShallow  = _ShallowColor;
                    half4 wDeep     = _DeepColor;
                    half4 wFoam     = _FoamColor;
                    half3 wCaustics = _CausticsColor.rgb;
                    half3 wDark     = _DarkColor.rgb;
                #endif

                // ── Ripples modulate depth-blend instead of adding to RGB ──
                // This removes the "liquid mercury" metallic shimmer effect.
                // Ripples shift the perceived depth → gentle color variation.
                float rippleN    = Ripples(IN.positionWS.xz, _Time.y);
                float rippleFade = saturate(fade + (rippleN - 0.5) * _RippleStr * shallowMask);

                // ── Base water colour ─────────────────────────────────────
                half4 col = lerp(wShallow, wDeep, rippleFade);

                // ── Underwater refraction ─────────────────────────────────
                // Minimum opacity floor (0.45) prevents full transparency that
                // causes the mercury / liquid-metal look in shallow water.
                float2 uwOff  = UnderwaterOffset(IN.positionWS.xz, _Time.y) * saturate(diff * 3.0);
                half3  opaque = SampleSceneColor(baseUV + uwOff).rgb;

                float waterVis = max(lerp(_UnderwaterVisibility, 1.0, rippleFade), 0.45);
                col.rgb = lerp(opaque, col.rgb, waterVis);

                // ── Caustics ──────────────────────────────────────────────
                float causticZone = shallowMask * saturate(diff * 2.0);
                col.rgb += Caustics(IN.positionWS.xz, _Time.y) * _CausticsStr * causticZone * wCaustics;

                // ── Dark patches ──────────────────────────────────────────
                float darkZone = fade * saturate(diff * 0.8);
                col.rgb = lerp(col.rgb, wDark, DarkPatches(IN.positionWS.xz, _Time.y) * _DarkPatchStr * darkZone);

                // ── Texture blend ─────────────────────────────────────────
                float2 texUV   = IN.positionWS.xz * _BlendScale * 0.1
                               + float2(_BlendScrollX, _BlendScrollY) * _Time.y;
                half4  texSamp = SAMPLE_TEXTURE2D(_BlendTex, sampler_BlendTex, texUV);
                float  texMask = dot(texSamp.rgb, float3(0.299, 0.587, 0.114));

                #if defined(_BLENDMODE_MULTIPLY)
                    col.rgb = col.rgb * lerp(1.0, texSamp.rgb * _BlendColor.rgb * 2.0, _BlendStr);
                #elif defined(_BLENDMODE_TINT)
                    col.rgb = lerp(col.rgb, col.rgb * _BlendColor.rgb * 2.0, texMask * _BlendStr);
                #else
                    col.rgb = lerp(col.rgb, BlendOverlay(col.rgb, texSamp.rgb * _BlendColor.rgb), _BlendStr);
                #endif

                // ── Swamp layers ──────────────────────────────────────────
                #if defined(_SWAMP_ON)
                    col.rgb = lerp(col.rgb, _SwampTurbidColor.rgb, _SwampTurbidity * 0.6);
                    col.a   = saturate(col.a + _SwampOpacity * 0.5);

                    #if defined(_SWAMP_SCUM)
                        float scum = SwampScum(IN.positionWS.xz, _Time.y) * lerp(1.0, 0.4, fade);
                        col.rgb = lerp(col.rgb, _SwampScumColor.rgb, scum * _SwampScumStr);
                        col.a   = max(col.a, scum * _SwampScumStr * 0.7);
                    #endif

                    #if defined(_SWAMP_ALGAE)
                        float algae = SwampAlgae(IN.positionWS.xz, _Time.y) * (shallowMask * 0.9 + 0.1);
                        col.rgb = lerp(col.rgb, _SwampAlgaeColor.rgb, algae * _SwampAlgaeStr);
                    #endif

                    #if defined(_SWAMP_BUBBLES)
                        float bubbles = SwampBubbles(IN.positionWS.xz, _Time.y) * shallowMask;
                        col.rgb += _SwampBubbleColor.rgb * bubbles * _SwampBubbleStr * 0.6;
                        col.a   = max(col.a, bubbles * _SwampBubbleStr * 0.5);
                    #endif

                    #if defined(_SWAMP_SILT)
                        float silt = SwampSilt(IN.positionWS.xz, _Time.y) * fade;
                        col.rgb = lerp(col.rgb, _SwampSiltColor.rgb, silt * _SwampSiltStr);
                    #endif
                #endif

                // ── FOAM ──────────────────────────────────────────────────

                // 1. Subtle reach variation — ±12% to avoid inconsistent rings
                float foamReach = VNoise(IN.positionWS.xz * 0.60
                                + float2(_Time.y * 0.06, _Time.y * 0.04));
                float modDepth  = _FoamDepth * lerp(0.88, 1.12, foamReach);
                float shoreGrad = 1.0 - saturate(diff / max(0.001, modDepth));

                // 2. Distort + flow offset
                float2 distOff   = FoamDistortUV(IN.positionWS.xz, _Time.y) * shoreGrad;

                float2 flowDir2D = float2(_FoamFlowX, _FoamFlowZ);
                float  flowMag   = length(flowDir2D);
                float2 flowN     = (flowMag > 0.001) ? (flowDir2D / flowMag) : float2(1.0, 0.0);
                float  flowStr   = saturate(flowMag * 2.0);
                float2 flowOff   = flowN * (1.0 - shoreGrad) * flowMag * _FoamDepth * 1.5;
                float2 foamXZ    = IN.positionWS.xz + distOff + flowOff;

                // 3. Base FBM + directional streak noise
                float noiseBase    = FoamFBM(foamXZ, _Time.y);
                float2 flowPerp    = float2(-flowN.y, flowN.x);
                float2 streakUV    = float2(dot(foamXZ, flowN)   * 0.25,
                                            dot(foamXZ, flowPerp) * 3.50)
                                   + flowN * _Time.y * _FoamDistortSpeed * 2.0;
                float  noiseStreak = FoamFBM(streakUV, 0.0);
                float  foamNoise   = lerp(noiseBase, noiseStreak, flowStr);

                // 4. Variation breakup — protected near objects.
                //    shoreGrad is high (1) right at object → varFade=0 → varMask=1 (solid).
                //    Away from object → varFade=1 → variation applies.
                //    This fixes foam that "hides inside" the model edge.
                float breakupN  = VNoise(foamXZ * 5.5
                                + float2(_Time.y * 0.28, _Time.y * 0.19));
                float varFade   = saturate((1.0 - shoreGrad) * 3.0);
                float varMask   = lerp(1.0, smoothstep(0.30, 0.68, breakupN),
                                       _FoamVariation * varFade);

                // 5. Foam mask
                //    S-curve for balanced gradient distribution
                float shoreS   = shoreGrad * shoreGrad * (3.0 - 2.0 * shoreGrad);
                float noiseMod = (foamNoise - 0.5) * 0.28;
                float thresh   = 0.50 + noiseMod;
                float innerW   = lerp(0.22, 0.05, _FoamSharpness);
                float outerW   = 0.04;
                float fMask    = smoothstep(thresh - innerW, thresh + outerW, shoreS) * varMask;

                col   = lerp(col, wFoam, fMask * wFoam.a);
                col.a = max(col.a, fMask * 0.9);

                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
