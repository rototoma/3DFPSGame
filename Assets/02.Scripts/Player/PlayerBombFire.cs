using System;
using Lean.Pool;
using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    private float _fireCharge = 0f;
    private bool _isCharging = false;
    private float _throwPower = 10f;
    [SerializeField] private float _chargeMultiplier = 10f;
    [SerializeField] private int _maxBombs = 10;

    private int _currentBomb = 0;
    private float _bombRigidbodyMass = 1f;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private Bomb _bombPrefab;
    private Animator _animator; // 군인 모델의 애니메이터

    public int CurrentBomb => _currentBomb;
    public int MaxBombs => _maxBombs;

    // 폭탄을 던질 준비(충전) 중인지. 궤적 미리보기가 표시 여부를 판단하는 데 쓴다.
    public bool IsAiming => _isCharging
                            && PlayerManager.Instance.PlayerWeaponMode == WeaponMode.Normal
                            && _currentBomb < _maxBombs;
    public Transform FirePoint => _firePoint;

    // 지금 충전량으로 던졌을 때 폭탄의 초기 속도 (Impulse를 질량으로 나눈 값)
    public Vector3 GetThrowVelocity()
    {
        float impulse = _fireCharge * _chargeMultiplier + _throwPower;
        return impulse / _bombRigidbodyMass * Camera.main.transform.forward;
    }

    // 옵저버: 폭탄 수가 바뀌면 (현재, 최대)를 구독자에게 알린다.
    public event Action<int, int> BombCountChanged;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _bombRigidbodyMass = _bombPrefab.GetComponent<Rigidbody>().mass;
    }

    private void Update()
    {
        // Game Play상태가 아니라면 연산x
        if (GameManager.Instance.GameState != GameState.Playing) return;
        // 1. 마우스 오른쪽 버튼 입력 받기
        if (Input.GetButtonDown("Fire2"))
        {
            _isCharging = true;
        }

        if (Input.GetButtonUp("Fire2") && PlayerManager.Instance.PlayerWeaponMode == WeaponMode.Normal)
        {
            Shoot(_fireCharge);
            _animator.SetTrigger("Throw"); // 군인이 쏘는 동작을 한다.
            _isCharging = false;
            _fireCharge = 0;
        }

        if (_isCharging)
        {
            _fireCharge += Time.deltaTime;
        }
    }

    private void Shoot(float fireCharge)
    {
        if (_currentBomb >= _maxBombs) return;

        Bomb bomb = LeanPool.Spawn(_bombPrefab, _firePoint.position, Quaternion.identity);
        bomb.Init(this);
        _currentBomb++;
        BombCountChanged?.Invoke(_currentBomb, _maxBombs);

        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();

        // 풀에서 재사용된 폭탄은 이전 비행/충돌의 속도와 회전이 남아 있으므로 초기화한 뒤 힘을 준다.
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        rigidbody.AddForce((fireCharge * _chargeMultiplier + _throwPower) * Camera.main.transform.forward,
            ForceMode.Impulse);
    }

    public void BombDestroyed()
    {
        _currentBomb--;
        BombCountChanged?.Invoke(_currentBomb, _maxBombs);
    }
}