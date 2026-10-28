using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpPower = 7f;
    [SerializeField] private float _runSpeed = 12f;
    [SerializeField, Min(1f)] private float _fallGravityMultiplier = 1.6f;
    [SerializeField, Min(0f)] private float _coyoteTime = 0.1f;
    [SerializeField, Min(0f)] private float _jumpBufferTime = 0.1f;

    public float Gravity = -20f; // 중력 가속도
    private float _yVelocity = 0f; // 위아래 속도. 매 프레임 중력이 쌓인다.
    private bool _isDashing = false;
    private int _jumpLimit = 2;
    private int _jumpCount = 0;
    private float _lastGroundedTime = float.NegativeInfinity;
    private float _lastJumpPressedTime = float.NegativeInfinity;

    private Animator _animator; // 군인 모델의 동작을 바꿔 주는 컴포넌트

    private void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }

        _animator = GetComponentInChildren<Animator>(); // 자식인 Model에 붙어 있다.
    }

    private void Update()
    {
        // Game Play상태가 아니라면 연산x
        if (GameManager.Instance.GameState != GameState.Playing) return;

        // GetAxis: 0~1사이로 보정이 들어감. GetAxisRaw: 키를 누르면 즉시, 떼면 즉시 반영
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        _isDashing = isShiftPressed && PlayerManager.Instance.PlayerStamina > 0f;
        if (_isDashing)
        {
            PlayerManager.Instance.UseStamina(Time.deltaTime);
        }

        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();
        direction = transform.TransformDirection(direction); // 로컬 좌표계
        _animator.SetFloat("MoveMotion", direction.magnitude); // 움직이면 1, 멈추면 0

        // 3. 바닥에 서 있으면 떨어지는 속도를 초기화하고 점프횟수도 초기화
        if (_controller.isGrounded && _yVelocity <= 0f)
        {
            ResetGroundedState();
        }

        // 발판에서 걸어 떨어져 유예 시간이 지나면 지상 점프 기회는 소모한다.
        if (!_controller.isGrounded && Time.time - _lastGroundedTime > _coyoteTime)
        {
            _jumpCount = Mathf.Max(_jumpCount, 1);
        }

        if (Input.GetButtonDown("Jump"))
        {
            _lastJumpPressedTime = Time.time;
        }

        TryBufferedJump();

        // 상승 높이는 유지하고, 하강할 때만 중력을 더 강하게 적용한다.
        float gravityMultiplier = _yVelocity < 0f ? _fallGravityMultiplier : 1f;
        _yVelocity += Gravity * gravityMultiplier * Time.deltaTime;

        // 수평 속도와 수직 속도를 합쳐서 이동한다.
        Vector3 velocity = _isDashing ? direction * _runSpeed : direction * _moveSpeed;
        velocity.y = _yVelocity;
        _controller.Move(velocity * Time.deltaTime);

        // 이번 이동에서 착지했다면 저장된 입력을 즉시 소모한다.
        if (_controller.isGrounded && _yVelocity <= 0f)
        {
            ResetGroundedState();
            TryBufferedJump();
        }
    }

    private void ResetGroundedState()
    {
        _yVelocity = -2f; // 바닥 접촉을 유지하도록 살짝 아래로 누른다.
        _jumpCount = 0;
        _lastGroundedTime = Time.time;
    }

    private void TryBufferedJump()
    {
        if (Time.time - _lastJumpPressedTime > _jumpBufferTime || _jumpCount >= _jumpLimit)
        {
            return;
        }

        if (PlayerManager.Instance.UseStamina(0.5f))
        {
            _yVelocity = _jumpPower;
            _jumpCount++;
            _lastJumpPressedTime = float.NegativeInfinity;
            _lastGroundedTime = float.NegativeInfinity;
        }
    }
}