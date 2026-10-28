using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom inspector for Custom/EmaceArt/URP_ArtWater.
///
/// Per-mode memory for Cel Shading toggle:
///   - Toggling ON  → restores last Cel-mode  Steps/Bias (or built-in defaults)
///   - Toggling OFF → restores last Grad-mode Steps/Bias (or built-in defaults)
///   - Manual edits in either mode are saved automatically to the material asset.
///
/// Storage uses four [HideInInspector] float properties declared in the shader.
/// Sentinel value -1 means "never been saved" → use built-in defaults.
/// </summary>
public class ArtWaterEditor : ShaderGUI
{
    // ── Built-in defaults per mode ────────────────────────────────────────────
    private const float CEL_STEPS_DEFAULT  = 3f;
    private const float CEL_BIAS_DEFAULT   = 1.0f;

    private const float GRAD_STEPS_DEFAULT = 4f;
    private const float GRAD_BIAS_DEFAULT  = 0.5f;

    // ── Hidden storage property names ─────────────────────────────────────────
    private const string MEM_CEL_STEPS  = "_Mem_Cel_Steps";
    private const string MEM_CEL_BIAS   = "_Mem_Cel_Bias";
    private const string MEM_GRAD_STEPS = "_Mem_Grad_Steps";
    private const string MEM_GRAD_BIAS  = "_Mem_Grad_Bias";

    // ── Sentinel ──────────────────────────────────────────────────────────────
    private const float UNSET = -1f;

    // ── Track previous toggle state to detect flips ───────────────────────────
    private static readonly System.Collections.Generic.Dictionary<Material, bool>
        s_PrevCelState = new System.Collections.Generic.Dictionary<Material, bool>();

    // ─────────────────────────────────────────────────────────────────────────
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
    {
        Material mat = editor.target as Material;
        if (mat == null) { base.OnGUI(editor, props); return; }

        bool prevCel = s_PrevCelState.TryGetValue(mat, out bool p) ? p
                       : mat.GetFloat("_CelEnabled") > 0.5f;

        base.OnGUI(editor, props);

        bool nowCel = mat.GetFloat("_CelEnabled") > 0.5f;

        if (prevCel != nowCel)
            OnToggleFlipped(mat, from: prevCel, to: nowCel);

        s_PrevCelState[mat] = nowCel;
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static void OnToggleFlipped(Material mat, bool from, bool to)
    {
        float curSteps = mat.GetFloat("_CelSteps");
        float curBias  = mat.GetFloat("_CelBias");

        Undo.RecordObject(mat, "ArtWater: Toggle Cel Shading");

        if (to) // OFF → ON  (gradient → cel)
        {
            mat.SetFloat(MEM_GRAD_STEPS, curSteps);
            mat.SetFloat(MEM_GRAD_BIAS,  curBias);

            float savedSteps = mat.GetFloat(MEM_CEL_STEPS);
            float savedBias  = mat.GetFloat(MEM_CEL_BIAS);
            mat.SetFloat("_CelSteps", IsSet(savedSteps) ? savedSteps : CEL_STEPS_DEFAULT);
            mat.SetFloat("_CelBias",  IsSet(savedBias)  ? savedBias  : CEL_BIAS_DEFAULT);
        }
        else    // ON → OFF  (cel → gradient)
        {
            mat.SetFloat(MEM_CEL_STEPS, curSteps);
            mat.SetFloat(MEM_CEL_BIAS,  curBias);

            float savedSteps = mat.GetFloat(MEM_GRAD_STEPS);
            float savedBias  = mat.GetFloat(MEM_GRAD_BIAS);
            mat.SetFloat("_CelSteps", IsSet(savedSteps) ? savedSteps : GRAD_STEPS_DEFAULT);
            mat.SetFloat("_CelBias",  IsSet(savedBias)  ? savedBias  : GRAD_BIAS_DEFAULT);
        }

        EditorUtility.SetDirty(mat);
    }

    private static bool IsSet(float v) => v > UNSET + 0.5f;
}
