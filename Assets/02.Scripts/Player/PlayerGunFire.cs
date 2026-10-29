using System;
using System.Collections;
using UnityEngine;

// 좌클릭으로 총 발사
public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _originPoint;
    [SerializeField] private ParticleSystem _hitEffectPrefab;
    [SerializeField] private int _maxBulletCount = 30;
    [SerializeField] private float _reloadTime = 3f;
    [SerializeField] private Coroutine _reloadCoroutine = null;

    [Header("궤적 (LineRenderer)")]
    [SerializeField] private Material _tracerMaterial; // 비워 두면 기본 Unlit 머티리얼을 만든다.
    [SerializeField] private Color _tracerColor = Color.yellow;
    [SerializeField] private float _tracerWidth = 0.03f;
    [SerializeField] private float _tracerDuration = 0.2f; // 마우스를 뗀 뒤 선이 남아 있는 시간 (초)
    [SerializeField] private Vector3 _tracerStartOffset = new Vector3(0.2f, -0.2f, 0.5f);

    [SerializeField] private GameObject[] _muzzleFlashes;
    [SerializeField] private float _flashTime;

    private LineRenderer _tracer;
    private float _tracerTimer;

    private float _range = 100f;
    private float _damage = 10f;
    private float _fireInterval = 0.1f;
    private float _fireCoolDown = 0;
    private int _bulletCount = 0;

    public int BulletCount => _bulletCount;
    public int MaxBulletCount => _maxBulletCount;

    // 옵저버: 총알 수가 바뀌면 (현재, 최대)를 구독자에게 알린다.
    public event Action<int, int> BulletCountChanged;

    private Animator _animator; // 군인 모델의 애니메이터

    private void Awake()
    {
        SetUpTracer();
        _bulletCount = _maxBulletCount;
        _animator = GetComponentInChildren<Animator>();
    }

    // 총알이 날아간 선을 그릴 LineRenderer를 준비한다. 평소에는 꺼 둔다.
    private void SetUpTracer()
    {
        _tracer = GetComponent<LineRenderer>();
        if (_tracer == null)
        {
            _tracer = gameObject.AddComponent<LineRenderer>();
        }

        _tracer.positionCount = 2;
        _tracer.enabled = false;
    }

    private void ShowTracer(Vector3 start, Vector3 end)
    {
        _tracer.SetPosition(0, start);
        _tracer.SetPosition(1, end);
        _tracer.enabled = true;
        _tracerTimer = _tracerDuration;
    }


    private void Update()
    {
        // 궤적: 마우스를 떼면 _tracerDuration 동안만 더 보여 주고 끈다.
        if (_tracer.enabled)
        {
            _tracerTimer -= Time.deltaTime;
            if (_tracerTimer <= 0f)
            {
                _tracer.enabled = false;
            }
        }

        // Game Play상태가 아니라면 연산x
        if (GameManager.Instance.GameState != GameState.Playing) return;
        _fireCoolDown -= Time.deltaTime;

        // 1. 마우스 왼쪽 버튼을 누르고 있는 동안은 매 프레임 조준선을 갱신하고, 발사 간격마다 쏜다.
        if (Input.GetButton("Fire1") && _bulletCount > 0)
        {
            Fire();
            if (_animator.GetFloat("MoveMotion") == 0f)
            {
                _animator.SetBool("Fire", true); // 군인이 쏘는 동작을 한다.
            }
        }
        else
        {
            _animator.SetBool("Fire", false);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_reloadCoroutine == null)
            {
                _reloadCoroutine = StartCoroutine(Co_Reload());
            }
        }
    }

    private void Fire()
    {
        // Ray: 레이저 (시작위치, 날아갈 방향)
        // Physics.RayCast: 레이저를 발사
        // RayCastHit: 레이저가 물체와 부딪히면 호출

        // 2. Ray를 생성하고 발사될 위치와 진행 방향 설정하기
        Transform cameraTr = Camera.main.transform;
        Ray ray = new Ray(cameraTr.position, cameraTr.forward);

        // 3. Ray가 부딪힌 대상의 정보를 저장할 변수 생성하기
        RaycastHit hitInfo = new RaycastHit();
        bool isHit = Physics.Raycast(ray, out hitInfo, _range);

        // 선은 총구(없으면 카메라 앞쪽 아래)에서 맞은 지점(빗나가면 사거리 끝)까지 그린다.
        Vector3 tracerStart = _firePoint != null
            ? _firePoint.position
            : cameraTr.TransformPoint(_tracerStartOffset);
        Vector3 tracerEnd = isHit ? hitInfo.point : ray.origin + ray.direction * _range;
        ShowTracer(tracerStart, tracerEnd);

        if (_fireCoolDown > 0) return;

        _bulletCount--;
        BulletCountChanged?.Invoke(_bulletCount, _maxBulletCount);
        StartCoroutine(Co_MuzzleFlash());
        _fireCoolDown = _fireInterval;

        // 4. 부딪힌 물체가 있다면 이펙트 표시하고 대미지 주기
        if (!isHit) return;

        ParticleSystem effect = Lean.Pool.LeanPool.Spawn(_hitEffectPrefab);
        effect.transform.position = hitInfo.point;
        effect.transform.forward = hitInfo.normal;
        effect.Play();

        DamageInfo damageInfo = new DamageInfo(_damage, hitInfo.point, hitInfo.normal);
        hitInfo.collider.GetComponent<IDamageable>()?.TakeDamage(damageInfo);
    }

    private IEnumerator Co_Reload()
    {
        yield return new WaitForSeconds(_reloadTime);

        _bulletCount = _maxBulletCount;
        BulletCountChanged?.Invoke(_bulletCount, _maxBulletCount);
        _reloadCoroutine = null;
    }

    private IEnumerator Co_MuzzleFlash()
    {
        int randomIndex = UnityEngine.Random.Range(0, _muzzleFlashes.Length);
        GameObject flash = _muzzleFlashes[randomIndex];
        flash.SetActive(true);

        yield return new WaitForSeconds(_flashTime);
        flash.SetActive(false);
    }
}