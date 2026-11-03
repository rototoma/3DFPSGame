using UnityEngine;

// 로그인 씬 연출용 카메라.
// 플레이어 머리 위쪽에서 대각선 아래로 내려다보며, 플레이어 주위를 천천히 360도 회전해 맵을 보여준다.
public class LoginCameraOrbit : MonoBehaviour
{
    private const float FullCircle = 360f;

    [SerializeField] private Transform _target;                 // 바라볼 플레이어 (루트)
    [SerializeField] private float _lookAtHeight = 1.4f;        // 플레이어 발 기준, 카메라가 바라볼 높이 (머리 근처)
    [SerializeField] private float _heightAboveHead = 2.8f;     // 바라보는 지점보다 카메라가 위로 얼마나 떠 있는지
    [SerializeField] private float _radius = 6f;                // 플레이어와의 수평 거리
    [SerializeField] private float _degreesPerSecond = 10f;     // 회전 속도 (10이면 한 바퀴에 36초)
    [SerializeField] private float _startAngle = 0f;            // 시작 각도

    private float _angle;

    private void Start()
    {
        _angle = _startAngle;
        UpdateCamera();
    }

    private void LateUpdate()
    {
        _angle = Mathf.Repeat(_angle + _degreesPerSecond * Time.deltaTime, FullCircle);
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (_target == null) return;

        float rad = _angle * Mathf.Deg2Rad;
        Vector3 lookAtPoint = _target.position + Vector3.up * _lookAtHeight;
        Vector3 offset = new Vector3(Mathf.Sin(rad) * _radius, _heightAboveHead, Mathf.Cos(rad) * _radius);

        transform.position = lookAtPoint + offset;
        transform.LookAt(lookAtPoint);
    }
}
