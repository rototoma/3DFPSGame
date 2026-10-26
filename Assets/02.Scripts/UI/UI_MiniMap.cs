using UnityEngine;

public class UI_MiniMap : MonoBehaviour
{
    [SerializeField] private MinimapCamera _miniMapCamera;

    public void OnZoomInClicked()
    {
        _miniMapCamera.ZoomIn();
    }

    public void OnZoomOutClicked()
    {
        _miniMapCamera.ZoomOut();
    }
}