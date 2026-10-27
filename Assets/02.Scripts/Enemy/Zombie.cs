using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum ZombieState
{
    Idle,
    Roaming,
    Trace,
    Comeback,
    Attack,
    Hit,
    Death,
}

// 좀비 AI: 지금 상태에 따라 다른 행동을 한다. (유한 상태 머신)
// 외부에서는 Zombie 목록·체력 조회를 ZombieManager(싱글톤)를 통해 한다.
[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject _bloodSprayPrefab;
    [Header("상태")]
    [SerializeField] private ZombieState _state = ZombieState.Idle;

    [Header("스탯")]
    [SerializeField] private float _maxHealth = 50f; // 최대 체력
    [SerializeField] private float _damage = 10f; // 공격력
    [SerializeField] private float _moveSpeed = 4f; // 이동 속도 (m/s)
    [SerializeField] private float _roamingCoolDown = 1f; // roaming 시작까지 남은 시간

    [Header("거리 (m)")]
    [SerializeField] private float _detectDistance = 8f; // 플레이어를 발견하는 거리
    [SerializeField] private float _attackDistance = 1.5f; // 공격하는 거리
    [SerializeField] private float _comebackDistance = 15f; // 처음 자리에서 이만큼 멀어지면 돌아간다

    [Header("시간 (초)")]
    [SerializeField] private float _attackInterval = 2f; // 공격 간격
    [SerializeField] private float _hitTime = 0.3f; // 맞고 멈칫하는 시간
    [SerializeField] private float _deathTime = 2f; // 쓰러진 뒤 사라지기까지
    [SerializeField] private float _roamingCoolTime = 1f; // idle에서 roaming으로 전환하는 간격
    [SerializeField] private float _roamingMoveTimer = 3f;

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    public ZombieState State => _state;
    public float MaxHealth => _maxHealth;
    public float Health { get; private set; } // 현재 체력
    public event Action HealthChanged; // 체력이 바뀔 때마다 호출

    private Player _player;
    private Vector3 _startPosition; // 처음 자리
    private float _attackTimer; // 다음 공격까지 남은 시간
    private Coroutine _hitCoroutine; // 실행 중인 피격
    private Coroutine _roamingCoroutine; // 실행 중인 배회

    // 애니메이터 상태 이름 (Animator Controller의 State 이름과 같아야 한다)
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int AttackDelayHash = Animator.StringToHash("AttackDelay");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    private void OnEnable()
    {
        ZombieManager manager = ZombieManager.Instance;
        if (manager != null)
        {
            manager.Register(this);
        }
    }

    private void OnDisable()
    {
        ZombieManager manager = ZombieManager.Instance;
        if (manager != null)
        {
            manager.Unregister(this);
        }
    }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); // 태그로 플레이어를 찾는다.
        _startPosition = transform.position;
        SetHealth(_maxHealth);
    }

    private void Update()
    {
        // Game Play상태가 아니라면 연산x
        if (GameManager.Instance.GameState != GameState.Playing) return;
        // 지금 상태에 맞는 함수를 부른다.
        switch (_state)
        {
            case ZombieState.Idle:
                Idle();
                break;
            case ZombieState.Roaming:
                Roaming();
                break;
            case ZombieState.Trace:
                Trace();
                break;
            case ZombieState.Comeback:
                Comeback();
                break;
            case ZombieState.Attack:
                Attack();
                break;
        }
    }

    // 상태는 이 함수로만 바꾼다.
    private void ChangeState(ZombieState nextState)
    {
        Debug.Log($"{name}: {_state} -> {nextState}");
        _state = nextState;

        if (_roamingCoroutine != null)
        {
            StopCoroutine(_roamingCoroutine);
            _roamingCoroutine = null;
        }
    }

    // 대기: 플레이어가 발견 거리 안에 들어오면 쫓아간다.
    private void Idle()
    {
        _roamingCoolDown -= Time.deltaTime;

        if (DistanceToPlayer() <= _detectDistance)
        {
            ChangeState(ZombieState.Trace);
            _animator.Play(MoveHash);
            return;
        }

        // 로밍 쿨타임 돌면 로밍 ㄱㄱ
        if (_roamingCoolDown < 0)
        {
            ChangeState(ZombieState.Roaming);
            _animator.Play(MoveHash);
            _roamingCoolDown = _roamingCoolTime;
        }
    }

    private void Roaming()
    {
        if (_roamingCoroutine == null)
        {
            _roamingCoroutine = StartCoroutine(Roaming_Coroutine());
        }
    }

    // 추격: 플레이어 쪽으로 걸어간다.
    private void Trace()
    {
        // 처음 자리에서 너무 멀어졌으면 돌아간다.
        if (Vector3.Distance(transform.position, _startPosition) > _comebackDistance)
        {
            ChangeState(ZombieState.Comeback);
            return;
        }

        // 공격 거리 안이면 공격한다.
        if (DistanceToPlayer() <= _attackDistance)
        {
            _attackTimer = 0f; // 공격 상태가 되자마자 첫 공격을 한다.
            ChangeState(ZombieState.Attack);
            _animator.Play(AttackDelayHash);
            return;
        }

        _agent.SetDestination(_player.transform.position);
    }

    // 복귀: 처음 자리로 걸어간다. 도착하면 체력을 채우고 다시 기다린다.
    private void Comeback()
    {
        if (Vector3.Distance(transform.position, _startPosition) < _agent.stoppingDistance)
        {
            SetHealth(_maxHealth);
            ChangeState(ZombieState.Idle);
            _animator.Play(IdleHash);
            return;
        }

        _agent.SetDestination(_startPosition);
    }

    // 공격: 공격 간격마다 플레이어를 때린다.
    private void Attack()
    {
        StopAgent();
        // 플레이어가 공격 거리 밖으로 나가면 다시 쫓아간다.
        if (DistanceToPlayer() > _attackDistance)
        {
            ChangeState(ZombieState.Trace);
            _animator.Play(MoveHash);
            return;
        }

        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0f)
        {
            _attackTimer = _attackInterval;
            _animator.Play(AttackHash);
        }
    }


    private void StopAgent()
    {
        _agent.ResetPath(); // 에이전트가 움직이지 않도록 멈춤
        _agent.velocity = Vector3.zero; // 가속도를 0으로 (안 하면 미끄러짐)
    }

    // 대미지를 받는다. 이미 죽었으면 받지 않고 false를 돌려준다.
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_state == ZombieState.Death)
        {
            return;
        }

        SetHealth(Mathf.Max(0f, Health - damageInfo.Value));

        if (Health > 0f)
        {
            // 피격: 실행 중인 피격이 있으면 멈추고 처음부터 다시 시작한다.
            if (_hitCoroutine != null)
            {
                StopCoroutine(_hitCoroutine);
            }

            GameObject bloodSpray = Instantiate(_bloodSprayPrefab, gameObject.transform);
            bloodSpray.transform.position = damageInfo.HitPoint;
            bloodSpray.transform.forward = damageInfo.HitNormal;

            ChangeState(ZombieState.Hit);
            _hitCoroutine = StartCoroutine(Hit_Coroutine());
        }
        else
        {
            // 죽음: 하던 일(코루틴)을 모두 멈추고 쓰러진다.
            StopAllCoroutines();
            ChangeState(ZombieState.Death);
            ZombieManager.Instance.NotifyZombieDied(this); // 스폰된 수에서 빼고 새 좀비를 스폰 예정에 넣는다.
            StartCoroutine(Death_Coroutine());
        }

        return;
    }

    private IEnumerator Roaming_Coroutine()
    {
        // 주변 아무 방향이나 이동
        float xMove = UnityEngine.Random.Range(-2f, 2f);
        float zMove = UnityEngine.Random.Range(-2f, 2f);
        Vector3 roamingTarget = new Vector3(_startPosition.x + xMove, _startPosition.y, _startPosition.z + zMove);

        float timer = 0f;
        while (timer < _roamingMoveTimer)
        {
            _agent.SetDestination(roamingTarget);
            timer += Time.deltaTime;
            if ((roamingTarget - transform.position).magnitude < 0.1f)
            {
                break;
            }

            if (DistanceToPlayer() <= _detectDistance)
            {
                ChangeState(ZombieState.Trace);
                yield break;
            }

            yield return null;
        }

        _animator.Play(IdleHash);

        yield return new WaitForSeconds(1.0f);

        timer = 0f;
        _animator.Play(MoveHash); // Play는 호출할 때마다 처음부터 재생하므로 루프 밖에서 한 번만 부른다.
        while (timer < _roamingMoveTimer)
        {
            _agent.SetDestination(_startPosition);
            timer += Time.deltaTime;
            if ((roamingTarget - transform.position).magnitude < 0.1f)
            {
                break;
            }

            if (DistanceToPlayer() <= _detectDistance)
            {
                ChangeState(ZombieState.Trace);
                yield break;
            }

            yield return null;
        }

        ChangeState(ZombieState.Idle);
        _animator.Play(IdleHash);
    }

    private IEnumerator Hit_Coroutine()
    {
        StopAgent();
        _animator.Play(HitHash);
        yield return new WaitForSeconds(_hitTime); // 잠깐 멈칫한다.

        _hitCoroutine = null;
        ChangeState(ZombieState.Trace); // 맞았으니 쫓아간다.
        _animator.Play(MoveHash);
    }

    private IEnumerator Death_Coroutine()
    {
        StopAgent();
        _animator.Play(DeathHash);
        yield return new WaitForSeconds(_deathTime);
        Destroy(gameObject);
    }

    // 체력은 이 함수로만 바꾼다. 바뀌면 구독자에게 알린다.
    private void SetHealth(float health)
    {
        Health = health;
        HealthChanged?.Invoke();
    }

    // 플레이어까지의 거리
    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }

    // 목표 지점을 향해 바닥을 따라 걷는다.
    private void MoveTo(Vector3 target, float moveSpeed)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f; // 높이 차이는 빼고 바닥 방향만 본다.
        direction.Normalize();

        if (direction.magnitude != 0)
        {
            transform.forward = direction; // 가는 쪽을 바라본다.
        }
    }

    public void AttackAction()
    {
        var closest = _player.GetComponent<Collider>().ClosestPoint(transform.position);
        DamageInfo damageInfo =
            new DamageInfo(_damage, closest, (gameObject.transform.position - closest).normalized);
        _player.TakeDamage(damageInfo);
    }
}