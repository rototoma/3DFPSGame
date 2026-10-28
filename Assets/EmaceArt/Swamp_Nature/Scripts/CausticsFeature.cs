// ============================================================
//  EmaceArt — Caustics Renderer Feature
//  URP 12+ (Unity 2021.2+)
//
//  Setup: select your URP Renderer asset → Add Renderer Feature
//  → "EmaceArt / Caustics Feature"
//
//  The feature injects an additive fullscreen pass that reads
//  the camera depth, reconstructs world-space positions, and
//  draws a binary (hard-step) caustic pattern on geometry that
//  sits below the specified Water Level.
// ============================================================

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace EmaceArt
{
    [Serializable]
    public class CausticsSettings
    {
        [Header("Visuals")]
        [Range(0f, 3f)]    public float strength   = 0.8f;
        [Range(0f, 0.05f)] public float rgbSplit   = 0.008f;
        [Range(0f, 1f)]    public float shadowMask = 0f;     // reserved — future use

        [Header("Movement")]
        public Texture2D texture;                            // assign a caustics/noise texture
        [Range(0.1f, 10f)] public float scale = 4.0f;
        [Range(0f, 1f)]    public float speed = 0.08f;

        [Header("Depth")]
        public float waterLevel = 0f;                        // world-space Y of the water surface
        [Min(0.1f)] public float depth = 5f;                 // max depth below surface for caustics
        [Range(0.1f, 4f)] public float fade = 1f;            // falloff curve exponent

        [Header("Rendering")]
        public RenderPassEvent renderPassEvent =
            RenderPassEvent.AfterRenderingSkybox;            // renders before transparent water
    }

    // ──────────────────────────────────────────────────────────────────────────
    public class CausticsFeature : ScriptableRendererFeature
    {
        public CausticsSettings settings = new CausticsSettings();

        CausticsPass _pass;
        Material     _mat;

        // ── Create ────────────────────────────────────────────────────────────
        public override void Create()
        {
            var shader = Shader.Find("EmaceArt/CausticsEffect");
            if (shader == null)
            {
                Debug.LogWarning("[EmaceArt] CausticsFeature: shader 'EmaceArt/CausticsEffect' " +
                                 "not found. Make sure CausticsEffect.shader is in the project.");
                return;
            }
            _mat  = CoreUtils.CreateEngineMaterial(shader);
            _pass = new CausticsPass(settings, _mat)
            {
                renderPassEvent = settings.renderPassEvent
            };
        }

        // ── Enqueue ───────────────────────────────────────────────────────────
        public override void AddRenderPasses(ScriptableRenderer renderer,
                                             ref RenderingData   renderingData)
        {
            if (_pass == null || _mat == null) return;

            var camType = renderingData.cameraData.cameraType;
            if (camType == CameraType.Reflection || camType == CameraType.Preview) return;

            _pass.renderPassEvent = settings.renderPassEvent;
            renderer.EnqueuePass(_pass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_mat);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  INNER PASS
        // ══════════════════════════════════════════════════════════════════════
        sealed class CausticsPass : ScriptableRenderPass
        {
            readonly CausticsSettings _s;
            readonly Material         _mat;

            // Shader property IDs (cached once)
            static readonly int ID_Params1 = Shader.PropertyToID("_CausticsParams1");
            static readonly int ID_Params2 = Shader.PropertyToID("_CausticsParams2");
            static readonly int ID_Time    = Shader.PropertyToID("_CausticsTime");
            static readonly int ID_Tex     = Shader.PropertyToID("_CausticsTex");

            readonly ProfilingSampler _sampler = new ProfilingSampler("EmaceArt.Caustics");

            public CausticsPass(CausticsSettings s, Material mat)
            {
                _s   = s;
                _mat = mat;
            }

            // Ask URP to provide the depth texture for this pass
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData rd)
            {
                ConfigureInput(ScriptableRenderPassInput.Depth);
            }

            public override void Execute(ScriptableRenderContext context,
                                         ref RenderingData       renderingData)
            {
                if (_mat == null) return;

                var cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, _sampler))
                {
                    // ── Upload shader properties ──────────────────────────────
                    // Params1: (strength, scale, speed, rgbSplit)
                    _mat.SetVector(ID_Params1,
                        new Vector4(_s.strength, _s.scale, _s.speed, _s.rgbSplit));

                    // Params2: (waterLevel, depth, fade, shadowMask)
                    _mat.SetVector(ID_Params2,
                        new Vector4(_s.waterLevel, _s.depth, _s.fade, _s.shadowMask));

                    _mat.SetFloat(ID_Time, Time.time);

                    if (_s.texture != null)
                    {
                        _mat.SetTexture(ID_Tex, _s.texture);
                        _mat.EnableKeyword("_CAUSTICS_TEX");
                    }
                    else
                    {
                        _mat.DisableKeyword("_CAUSTICS_TEX");
                    }

                    // ── Additive draw onto camera color ───────────────────────
                    // Blend One One in shader = additive light on top of scene
                    cmd.SetRenderTarget(
                        renderingData.cameraData.renderer.cameraColorTargetHandle,
                        RenderBufferLoadAction.Load,
                        RenderBufferStoreAction.Store);

                    cmd.DrawMesh(RenderingUtils.fullscreenMesh,
                                 Matrix4x4.identity, _mat, 0, 0);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }
}
