using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class SwampArchitectureUpgradeInfo : MonoBehaviour
{
    public string assetUrl = "https://assetstore.unity.com/packages/3d/props/exterior/modular-swamp-environment-architecture-builder-pack-modulacraft--285300";

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (Application.isPlaying) return;

        EditorApplication.delayCall += () =>
        {
            if (this != null && gameObject != null)
            {
                Selection.activeGameObject = gameObject;

                System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor");
                if (inspectorType != null)
                {
                    EditorWindow.GetWindow(inspectorType, false, "Inspector", true);
                }
            }
        };
    }
#endif
}