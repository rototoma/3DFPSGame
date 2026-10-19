using UnityEngine;

// V 키로 1인칭 -> 3인칭 -> 쿼터뷰 -> 숄더뷰 -> 탑다운 순서로 전환한다.
[RequireComponent(typeof(CameraFollow))]
public class CameraViewSwitcher : MonoBehaviour
{
    [SerializeField] private KeyCode _switchKey = KeyCode.V;

    private CameraFollow _cameraFollow;

    private void Awake()
    {
        _cameraFollow = GetComponent<CameraFollow>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_switchKey))
        {
            switch (_cameraFollow.ViewMode)
            {
                case CameraViewMode.FirstPerson:
                    _cameraFollow.SetViewMode(CameraViewMode.ThirdPerson);
                    break;
                case CameraViewMode.ThirdPerson:
                    _cameraFollow.SetViewMode(CameraViewMode.QuarterView);
                    break;
                case CameraViewMode.QuarterView:
                    _cameraFollow.SetViewMode(CameraViewMode.ShoulderView);
                    break;
                case CameraViewMode.ShoulderView:
                    _cameraFollow.SetViewMode(CameraViewMode.TopDown);
                    break;
                case CameraViewMode.TopDown:
                    _cameraFollow.SetViewMode(CameraViewMode.FirstPerson);
                    break;
                default:
                    _cameraFollow.SetViewMode(CameraViewMode.FirstPerson);
                    break;
            }
        }
    }
}
