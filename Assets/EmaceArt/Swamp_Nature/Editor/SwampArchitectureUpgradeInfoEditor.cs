using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SwampArchitectureUpgradeInfo))]
public class SwampArchitectureUpgradeInfoEditor : Editor
{
    private Texture2D coverImage;
    private GUIStyle headlineStyle;
    private GUIStyle bodyTextStyle;
    
    private const string TargetUrl = "https://assetstore.unity.com/packages/3d/props/exterior/modular-swamp-environment-architecture-builder-pack-modulacraft--285300";

    private void OnEnable()
    {
        string[] guids = AssetDatabase.FindAssets("SwampCover t:Texture2D");
        if (guids != null && guids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            coverImage = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        }
    }

    public override void OnInspectorGUI()
    {
        if (headlineStyle == null)
        {
            headlineStyle = new GUIStyle(GUI.skin.label) 
            { 
                fontSize = 14, 
                fontStyle = FontStyle.Bold, 
                wordWrap = true, 
                richText = true,
                margin = new RectOffset(5, 5, 2, 2)
            };
        }
        if (bodyTextStyle == null)
        {
            bodyTextStyle = new GUIStyle(GUI.skin.label) 
            { 
                fontSize = 12, 
                wordWrap = true, 
                richText = true,
                margin = new RectOffset(5, 5, 2, 2)
            };
        }

        EditorGUILayout.Space(5);

        if (coverImage != null)
        {
            float imageAspect = (float)coverImage.width / coverImage.height;
            float maxWidth = Mathf.Min(EditorGUIUtility.currentViewWidth - 45f, coverImage.width);
            float calculatedHeight = maxWidth / imageAspect;

            Rect imageRect = GUILayoutUtility.GetRect(maxWidth, calculatedHeight);
            GUI.DrawTexture(imageRect, coverImage, ScaleMode.ScaleToFit);

            if (GUI.Button(imageRect, GUIContent.none, GUIStyle.none))
            {
                Application.OpenURL(TargetUrl);
            }
            EditorGUILayout.Space(10);
        }

        GUILayout.Label("<b>The Synergy of ModulaCraft Series</b>", headlineStyle);
        EditorGUILayout.Space(4);

        GUILayout.Label("Thank you for choosing our nature package! This pack is a fully independent, complete set containing all the high-quality environmental and flora models.", bodyTextStyle);
        EditorGUILayout.Space(6);

        GUILayout.Label("To unleash the <b>maximum potential</b> of your projects, combine these assets with our dedicated architectural package. Both systems are designed for full, seamless integration, allowing you to instantly complement the wild landscape with matching structures and buildings.", bodyTextStyle);
        EditorGUILayout.Space(6);

        GUILayout.Label("<b>PRO TIP: As an owner of this nature pack, you are eligible for a <color=#FF5555>65% DISCOUNT</color> Upgrade on the Asset Store! Complete your set with the architectural package at a significantly lower price.</b>", bodyTextStyle);
        EditorGUILayout.Space(15);

        GUIStyle mainButtonStyle = new GUIStyle(GUI.skin.button) 
        { 
            fontStyle = FontStyle.Bold, 
            fontSize = 12,
            wordWrap = true,
            padding = new RectOffset(10, 10, 10, 10)
        };

        if (GUILayout.Button("Get ModulaCraft Swamp Builder PRO - Architecture Mega Pack (65% OFF Upgrade)", mainButtonStyle, GUILayout.Height(45)))
        {
            Application.OpenURL(TargetUrl);
        }

        EditorGUILayout.Space(10);
    }
}