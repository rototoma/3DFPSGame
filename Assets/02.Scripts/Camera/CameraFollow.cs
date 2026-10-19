using UnityEngine;

public enum CameraViewMode
{
    FirstPerson,
    ThirdPerson,
    QuarterView,
    ShoulderView
}

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [SerializeField] private float _firstPersonHeight = 0.5f;

    [SerializeField] private float _thirdViewDistance = 5f;
    [SerializeField] private float _targetHeight = 0.5f;

    [SerializeField] private float _quarterViewDistance = 10f;
    [SerializeField] private float _quarterViewPitch = 45f;
    [SerializeField] private float _quarterViewYaw = 45f;

    [SerializeField] private float _shoulderViewDistance = 2.5f;
    [SerializeField] private float _shoulderViewHeight = 0.8f;
    [SerializeField] private float _shoulderViewOffset = 0.7f;

    public CameraViewMode ViewMode { get; private set; } = CameraViewMode.FirstPerson;

    public void SetViewMode(CameraViewMode viewMode)
    {
        ViewMode = viewMode;
    }

    private void Awake()
    {
        if (_target == null)
        {
            GameLogger.LogWarning("No camera target found");
        }
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        // 시점별 높이, 거리, 좌우 오프셋으로 카메라를 배치한다.
        // 기본: 1인칭 기준
        float height = _firstPersonHeight;
        float distance = 0f;
        float horizontalOffset = 0f;
        switch (ViewMode)
        {
            case CameraViewMode.ThirdPerson:
            {
                height = _targetHeight;
                distance = _thirdViewDistance;
                break;
            }
            case CameraViewMode.QuarterView:
            {
                transform.rotation = Quaternion.Euler(_quarterViewPitch, _quarterViewYaw, 0f);
                distance = _quarterViewDistance;
                break;
            }
            case CameraViewMode.ShoulderView:
            {
                height = _shoulderViewHeight;
                distance = _shoulderViewDistance;
                horizontalOffset = _shoulderViewOffset;
                break;
            }
        }

        Vector3 focusPosition = _target.position + Vector3.up * height;
        // 시선 기준 오른쪽으로 이동해 플레이어가 화면 왼쪽에 보이게 한다.
        transform.position = focusPosition - transform.forward * distance
                             + transform.right * horizontalOffset;
    }
}