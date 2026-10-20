using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;

    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpPower = 7f;
    [SerializeField] private float _runSpeed = 12f;

    public float Gravity = -20f; // 중력 가속도
    private float _yVelocity = 0f; // 위아래 속도. 매 프레임 중력이 쌓인다.
    private bool _isDashing = false;
    private int _jumpLimit = 2;
    private int _jumpCount = 0;

    private void Update()
    {
        // GetAxis: 0~1사이로 보정이 들어감. GetAxisRaw: 키를 누르면 즉시, 떼면 즉시 반영
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _isDashing = Input.GetKey(KeyCode.LeftShift) && PlayerManager.Instance.UseStamina(Time.deltaTime);

        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();
        direction = transform.TransformDirection(direction); // 로컬 좌표계

        // 3. 바닥에 서 있으면 떨어지는 속도를 초기화하고 점프횟수도 초기화
        if (_controller.isGrounded && _yVelocity < 0f)
        {
            _yVelocity = -2f; // 살짝 아래로 눌러 두어야 isGrounded가 계속 true로 유지된다.
            _jumpCount = 0;
        }

        // 4. 바닥에서 점프 키를 누르면 위로 속도를 준다.
        if (Input.GetButtonDown("Jump") && _jumpCount < _jumpLimit)
        {
            if (PlayerManager.Instance.UseStamina(0.5f))
            {
                _yVelocity = _jumpPower;
                _jumpCount++;
            }
        }

        // 중력을 누적한다.
        _yVelocity += Gravity * Time.deltaTime;

        // 수평 속도와 수직 속도를 합쳐서 이동한다.
        Vector3 velocity = _isDashing ? direction * _runSpeed : direction * _moveSpeed;
        velocity.y = _yVelocity;
        _controller.Move(velocity * Time.deltaTime);

        if (_isDashing)
        {
            PlayerManager.Instance.UseStamina(Time.deltaTime);
        }
    }
}