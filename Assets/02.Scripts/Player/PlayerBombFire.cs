using System;
using Lean.Pool;
using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    private float _fireCharge = 0f;
    private bool _isCharging = false;
    private float _throwPower = 10f;
    [SerializeField] private float _chargeMultiplier = 5f;
    [SerializeField] private int _maxBombs = 10;

    private int _currentBomb = 0;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private Bomb _bombPrefab;
    private Animator _animator; // 군인 모델의 애니메이터

    public int CurrentBomb => _currentBomb;
    public int MaxBombs => _maxBombs;

    // 옵저버: 폭탄 수가 바뀌면 (현재, 최대)를 구독자에게 알린다.
    public event Action<int, int> BombCountChanged;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
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
        rigidbody.AddForce((fireCharge * _chargeMultiplier + _throwPower) * Camera.main.transform.forward,
            ForceMode.Impulse);
    }

    public void BombDestroyed()
    {
        _currentBomb--;
        BombCountChanged?.Invoke(_currentBomb, _maxBombs);
    }
}