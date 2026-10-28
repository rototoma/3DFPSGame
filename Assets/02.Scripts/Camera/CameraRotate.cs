using UnityEngine;


// 마우스로 플레이어를 바라보는 3인칭 카메라의 방향을 조절한다.
public class CameraRotate : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private float _initialPitch = 15f;
    [SerializeField] private float _minPitch = -90f;
    [SerializeField] private float _maxPitch = 90f;
    private float _accumulationX = 0f; // 좌우 회전각
    private float _accumulationY = 0f; // 상하 회전각

    private void Start()
    {
        _accumulationX = transform.eulerAngles.y;
        _accumulationY = Mathf.Clamp(_initialPitch, _minPitch, _maxPitch);
        transform.rotation = Quaternion.Euler(_accumulationY, _accumulationX, 0f);
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _accumulationX += mouseX * _rotateSpeed * Time.deltaTime;
        _accumulationY += -mouseY * _rotateSpeed * Time.deltaTime;

        _accumulationY = Mathf.Clamp(_accumulationY, _minPitch, _maxPitch);

        transform.rotation = Quaternion.Euler(_accumulationY, _accumulationX, 0f);
    }
}