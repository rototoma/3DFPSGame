using UnityEngine;
using UnityEngine.UI;

// 체력바가 줄어들 때 쫀득한 연출을 준다. 슬라이더에 붙여서 사용한다.
//  - 채움(Fill): 스프링으로 목표값을 따라가며 살짝 출렁인다.
//  - 잔상(Trail): 잠시 머문 뒤 일정한 속도로 줄어들어, 얼마나 깎였는지 보여준다.
//  - 피격 순간: 바가 통통 튀어나왔다가 돌아오고, 채움이 잠깐 번쩍인다.
// 슬라이더의 value만 바꾸는 기존 스크립트는 그대로 두고, 채움 표시는 이 컴포넌트가 대신 그린다.
// (슬라이더의 Fill Rect는 비워 두어야 이 컴포넌트와 충돌하지 않는다.)
public class UI_SmoothBar : MonoBehaviour
{
    private const float MaxDeltaTime = 0.05f;      // 프레임이 튀어도 스프링이 발산하지 않게 제한
    private const float SnapPositionEpsilon = 0.0005f;
    private const float SnapVelocityEpsilon = 0.001f;
    private const float DropEpsilon = 0.0001f;

    [SerializeField] private Slider _slider;
    [SerializeField] private RectTransform _fill;   // 앞쪽 채움 (anchorMax.x로 길이를 그린다)
    [SerializeField] private RectTransform _trail;  // 뒤쪽 잔상 (Fill 뒤에 그려진다)
    [SerializeField] private Image _fillImage;

    [Header("채움 스프링")]
    [SerializeField] private float _stiffness = 140f; // 클수록 빨리 따라간다
    [SerializeField] private float _damping = 15f;    // 작을수록 더 출렁인다

    [Header("잔상")]
    [SerializeField] private float _trailDelay = 0.15f; // 피격 후 잔상이 줄기 시작하기까지 대기 시간
    [SerializeField] private float _trailSpeed = 3f;    // 초당 줄어드는 비율

    [Header("피격 연출")]
    [SerializeField] private float _punchStiffness = 300f;
    [SerializeField] private float _punchDamping = 12f;
    [SerializeField] private float _punchPerRatio = 8f; // 깎인 비율에 곱하는 튕김 세기
    [SerializeField] private float _minPunch = 1.2f;
    [SerializeField] private float _maxPunch = 3f;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashDuration = 0.15f;

    private float _front;          // 화면에 그려지는 채움 비율
    private float _frontVelocity;
    private float _trailValue;     // 화면에 그려지는 잔상 비율
    private float _trailTimer;
    private float _lastTarget;
    private float _scaleOffset;    // 튀어나온 정도 (0이면 원래 크기)
    private float _scaleVelocity;
    private float _flashTimer;
    private Color _baseColor;
    private bool _needSnap = true;

    private void Awake()
    {
        _baseColor = _fillImage.color;
    }

    // 풀링으로 재활성화될 때, 이전 상태(출렁임/잔상)가 남지 않도록 첫 프레임에 현재 값으로 맞춘다.
    private void OnEnable()
    {
        _needSnap = true;
    }

    private void LateUpdate()
    {
        float target = _slider.normalizedValue;

        if (_needSnap)
        {
            Snap(target);
            _needSnap = false;
        }

        float deltaTime = Mathf.Min(Time.deltaTime, MaxDeltaTime);

        if (target < _lastTarget - DropEpsilon)
        {
            OnDropped(_lastTarget - target);
        }

        _lastTarget = target;

        UpdateFront(target, deltaTime);
        UpdateTrail(target, deltaTime);
        UpdatePunch(deltaTime);
        UpdateFlash(deltaTime);
        Apply();
    }

    private void Snap(float target)
    {
        _front = target;
        _frontVelocity = 0f;
        _trailValue = target;
        _trailTimer = 0f;
        _lastTarget = target;
        _scaleOffset = 0f;
        _scaleVelocity = 0f;
        _flashTimer = 0f;
    }

    private void OnDropped(float dropRatio)
    {
        _trailTimer = _trailDelay;
        _scaleVelocity += Mathf.Clamp(dropRatio * _punchPerRatio, _minPunch, _maxPunch);
        _flashTimer = _flashDuration;
    }

    private void UpdateFront(float target, float deltaTime)
    {
        _frontVelocity += (target - _front) * _stiffness * deltaTime;
        _frontVelocity -= _frontVelocity * Mathf.Min(1f, _damping * deltaTime);
        _front += _frontVelocity * deltaTime;

        if (Mathf.Abs(target - _front) < SnapPositionEpsilon && Mathf.Abs(_frontVelocity) < SnapVelocityEpsilon)
        {
            _front = target;
            _frontVelocity = 0f;
        }
    }

    private void UpdateTrail(float target, float deltaTime)
    {
        // 회복하거나 채움이 잔상을 밀어 올리면 잔상은 채움을 따라간다.
        if (_trailValue < _front)
        {
            _trailValue = _front;
            return;
        }

        _trailTimer -= deltaTime;
        if (_trailTimer <= 0f)
        {
            _trailValue = Mathf.MoveTowards(_trailValue, target, _trailSpeed * deltaTime);
        }
    }

    private void UpdatePunch(float deltaTime)
    {
        _scaleVelocity -= _scaleOffset * _punchStiffness * deltaTime;
        _scaleVelocity -= _scaleVelocity * Mathf.Min(1f, _punchDamping * deltaTime);
        _scaleOffset += _scaleVelocity * deltaTime;

        if (Mathf.Abs(_scaleOffset) < SnapPositionEpsilon && Mathf.Abs(_scaleVelocity) < SnapVelocityEpsilon)
        {
            _scaleOffset = 0f;
            _scaleVelocity = 0f;
        }
    }

    private void UpdateFlash(float deltaTime)
    {
        if (_flashTimer > 0f) _flashTimer -= deltaTime;
    }

    private void Apply()
    {
        _fill.anchorMax = new Vector2(Mathf.Clamp01(_front), 1f);
        _trail.anchorMax = new Vector2(Mathf.Clamp01(_trailValue), 1f);

        transform.localScale = Vector3.one * (1f + _scaleOffset);

        float flash = _flashDuration > 0f ? Mathf.Clamp01(_flashTimer / _flashDuration) : 0f;
        _fillImage.color = Color.Lerp(_baseColor, _flashColor, flash);
    }
}
