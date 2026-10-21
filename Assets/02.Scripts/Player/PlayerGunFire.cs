using System;
using UnityEngine;

// 좌클릭으로 총 발사
public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _originPoint;
    [SerializeField] private ParticleSystem _hitEffectPrefab;
    [SerializeField] private int _maxBulletCount = 30;
    [SerializeField] private float _reloadTime = 3f;

    private float _range = 100f;
    private float _damage = 10f;
    private float _fireInterval = 0.1f;
    private float _fireCoolDown = 0;
    private int _bulletCount = 0;
    private float _reloadTimer = 0f;
    private bool _isReloading = false;

    private void Start()
    {
        _bulletCount = _maxBulletCount;
    }


    private void Update()
    {
        _fireCoolDown -= Time.deltaTime;

        if (_isReloading)
        {
            _reloadTimer += Time.deltaTime;
            if (_reloadTimer > _reloadTime)
            {
                _isReloading = false;
                _reloadTimer = 0;
                Reloading();
            }
        }

        // 1. 마우스 왼쪽 버튼 입력 받기
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !_isReloading)
        {
            _isReloading = true;
            _reloadTimer = 0;
        }
    }

    private void Shoot()
    {
        if (_fireCoolDown > 0 || _bulletCount <= 0) return;

        _bulletCount--;
        _fireCoolDown = _fireInterval;

        // Ray: 레이저 (시작위치, 날아갈 방향)
        // Physics.RayCast: 레이저를 발사
        // RayCastHit: 레이저가 물체와 부딪히면 호출

        // 2. Ray를 생성하고 발사될 위치와 진행 방향 설정하기
        Transform cameraTr = Camera.main.transform;
        Ray ray = new Ray(cameraTr.position, cameraTr.forward);

        // 3. Ray가 부딪힌 대상의 정보를 저장할 변수 생성하기
        RaycastHit hitInfo = new RaycastHit();

        // 4. 레이를 발사하고, 부딪힌 물체가 있다면 이펙트 표시하기
        bool isHit = Physics.Raycast(ray, out hitInfo, _range);
        if (isHit)
        {
            ParticleSystem effect = Lean.Pool.LeanPool.Spawn(_hitEffectPrefab);
            effect.transform.position = hitInfo.point;
            effect.transform.forward = hitInfo.normal;
            effect.Play();
        }
    }

    private void Reloading()
    {
        _bulletCount = _maxBulletCount;
    }
}