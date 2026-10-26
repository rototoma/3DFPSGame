using UnityEngine;

// 정해진 위치에서 좀비를 하나씩 스폰해 최대 수를 계속 유지한다. 스폰 예정/스폰된 수는 ZombieManager가 관리한다.
public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private Zombie _zombiePrefab;
    [SerializeField] private Transform[] _spawnPoints; // 비어 있으면 이 오브젝트의 위치에서 스폰한다.

    [Header("스폰 설정")]
    [SerializeField] private int _maxAliveCount = 5; // 동시에 유지할 좀비 수 (죽으면 다시 스폰된다)
    [SerializeField] private float _spawnInterval = 1f; // 스폰 간격 (초)

    private float _spawnTimer;

    private void Start()
    {
        ZombieManager.Instance.AddMaxCount(_maxAliveCount);
    }

    private void Update()
    {
        // Game Play상태가 아니라면 연산x
        if (GameManager.Instance.GameState != GameState.Playing) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer > 0f) return;

        // 스폰 예정인 좀비가 없거나 이미 너무 많이 살아 있으면 스폰하지 않는다.
        if (!ZombieManager.Instance.TryConsumeScheduledCount()) return;

        _spawnTimer = _spawnInterval;
        Spawn();
    }

    private void Spawn()
    {
        Transform point = _spawnPoints != null && _spawnPoints.Length > 0
            ? _spawnPoints[Random.Range(0, _spawnPoints.Length)]
            : transform;

        Instantiate(_zombiePrefab, point.position, point.rotation);
    }
}