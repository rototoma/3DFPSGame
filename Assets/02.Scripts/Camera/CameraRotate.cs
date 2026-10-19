using System;
using UnityEngine;


// 역할: 마우스를 움직이면 카메라를 그 방향으로 회전하고 싶다.
public class CameraRotate : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 200f;
    private float _accumulationX = 0f; // 좌우 회전각
    private float _accumulationY = 0f; // 상하 회전각

    private void Start()
    {
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Debug.Log($"mouseX: {mouseX}, mouseY: {mouseY}");
        //
        // Vector3 dir = new Vector3(-mouseY, mouseX, 0);
        //
        // Vector3 rotation = transform.eulerAngles;

        _accumulationX += mouseX * _rotateSpeed * Time.deltaTime;
        _accumulationY += -mouseY * _rotateSpeed * Time.deltaTime;

        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);
        // float rotationY = Math.Clamp(transform.eulerAngles.y + _accumulationX, -90f, 90f);

        transform.eulerAngles = new Vector3(_accumulationY, _accumulationX, 0);
    }
}