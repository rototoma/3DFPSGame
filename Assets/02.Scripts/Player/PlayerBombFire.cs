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

    private void Update()
    {
        // 1. 마우스 오른쪽 버튼 입력 받기
        if (Input.GetButtonDown("Fire2"))
        {
            _isCharging = true;
        }

        if (Input.GetButtonUp("Fire2"))
        {
            Shoot(_fireCharge);
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

        Bomb bomb = Lean.Pool.LeanPool.Spawn(_bombPrefab, _firePoint.position, Quaternion.identity);
        bomb.Init(this);
        _currentBomb++;

        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
        rigidbody.AddForce((fireCharge * _chargeMultiplier + _throwPower) * Camera.main.transform.forward,
            ForceMode.Impulse);
    }

    public void BombDestroyed()
    {
        _currentBomb--;
    }
}