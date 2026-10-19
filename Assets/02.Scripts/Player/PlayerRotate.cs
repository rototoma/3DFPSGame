using System;
using UnityEngine;


// 역할: 마우스를 움직이면 카메라를 그 방향으로 회전하고 싶다.
public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 200f;
    private float _accumulationX = 0f; // 좌우 회전각

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");

        _accumulationX += mouseX * _rotateSpeed * Time.deltaTime;
        // float rotationY = Math.Clamp(transform.eulerAngles.y + _accumulationX, -90f, 90f);

        transform.eulerAngles = new Vector3(0f, _accumulationX, 0f);
    }
}