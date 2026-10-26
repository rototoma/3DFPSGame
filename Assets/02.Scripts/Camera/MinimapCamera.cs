using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Camera _miniMapCamera;
    [SerializeField] private Transform _target;
    [SerializeField] private float _height = 8f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ZoomOut();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ZoomIn();
        }
    }

    private void LateUpdate()
    {
        // 1. 대상의 머리 위로 위치를 옮긴다
        transform.position = _target.position + (Vector3.up * _height);

        // 2. 대상이 쳐다보는 방향으로 카메라도 회전한다. 단, 위에서 아래로 내려다보는 것은 유지해야함
        transform.eulerAngles = new Vector3(90, _target.eulerAngles.y, _target.eulerAngles.z);
    }

    public void ZoomIn()
    {
        _miniMapCamera.orthographicSize--;
    }

    public void ZoomOut()
    {
        _miniMapCamera.orthographicSize++;
    }
}