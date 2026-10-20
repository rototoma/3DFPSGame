using UnityEngine;

// 키보드(WASD)로 이동하고, 질주와 2단 점프에 스태미나를 쓴다.
public class PlayerMove : MonoBehaviour
{
    private const float Gravity = -20f;
    private float _yVelocity = 0f;
    private int _jumpCount = 0;

    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _sprintSpeed = 12f;
    [SerializeField] private float _jumpPower = 10f;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _stamina = 100f;
    [SerializeField] private float _sprintCost = 20f; // 1초에 쓰는 양
    [SerializeField] private float _jumpCost = 20f; // 한 번 뛸 때 쓰는 양
    [SerializeField] private float _staminaRecovery = 15f; // 1초에 회복하는 양

    public float MaxStamina => _maxStamina;
    public float Stamina => _stamina;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _stamina = _maxStamina;
    }

    private void Update()
    {
        // 1. 입력으로 방향을 만들고, 플레이어가 보는 방향 기준으로 바꾼다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, 0f, v);
        direction.Normalize();
        direction = transform.TransformDirection(direction);

        // 2. 바닥에 내려왔을 때 점프 횟수를 되돌린다.
        // 점프 직후에는 이전 Move의 isGrounded가 남을 수 있으므로 상승 중에는 리셋하지 않는다.
        if (_characterController.isGrounded && _yVelocity <= 0f)
        {
            _yVelocity = -1f;
            _jumpCount = 0;
        }
        else
        {
            _yVelocity += Gravity * Time.deltaTime;

            // 점프하지 않고 턱에서 떨어져도 공중 점프는 한 번만 남긴다.
            if (_jumpCount == 0)
            {
                _jumpCount = 1;
            }
        }

        // 3. 두 번까지 점프한다. 두 점프 모두 스태미나를 쓴다.
        bool jumped = false;
        if (Input.GetButtonDown("Jump") && _jumpCount < 2 && _stamina >= _jumpCost)
        {
            _yVelocity = _jumpPower;
            _jumpCount++;
            _stamina -= _jumpCost;
            jumped = true;
        }

        // 4. Shift를 누르고 움직일 때만 질주한다.
        bool wantsSprint = Input.GetButton("Sprint") && direction.sqrMagnitude > 0f;
        bool isSprinting = wantsSprint && _stamina > 0f;
        float currentSpeed = _moveSpeed;

        if (isSprinting)
        {
            currentSpeed = _sprintSpeed;
            _stamina -= _sprintCost * Time.deltaTime;
        }
        else if (!wantsSprint && !jumped)
        {
            _stamina += _staminaRecovery * Time.deltaTime;
        }

        // 스태미나가 0보다 작아지거나 최댓값을 넘지 않게 한다.
        _stamina = Mathf.Clamp(_stamina, 0f, _maxStamina);

        // 5. 수평 속도와 수직 속도를 합쳐서 이동한다.
        Vector3 velocity = direction * currentSpeed;
        velocity.y = _yVelocity;
        _characterController.Move(velocity * Time.deltaTime);
    }
}