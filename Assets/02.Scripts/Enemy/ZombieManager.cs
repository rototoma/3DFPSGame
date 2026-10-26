using System;
using System.Collections.Generic;
using UnityEngine;

// 씬의 모든 좀비를 관리하는 싱글톤: 외부에서는 Zombie를 직접 찾지 않고 이 클래스를 거친다.
public class ZombieManager : MonoBehaviour
{
    private static ZombieManager _instance;
    public static ZombieManager Instance => _instance;

    private List<Zombie> _zombies = new List<Zombie>();

    public int ScheduledCount { get; private set; } // 앞으로 스폰될 예정인 좀비 수
    public int SpawnedCount => _zombies.Count; // 지금 스폰되어 있는 좀비 수
    public int MaxCount { get; private set; } // 동시에 유지할 최대 좀비 수

    // 옵저버: 스폰된 수/최대 수가 바뀔 때마다 호출된다.
    public event Action ZombieCountChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    // 스포너가 시작할 때 유지할 좀비 수를 최대 수에 더하고, 그만큼을 스폰 예정에 넣는다.
    public void AddMaxCount(int count)
    {
        MaxCount += count;
        ScheduledCount += count;
        ZombieCountChanged?.Invoke();
    }

    // 스폰 예정인 좀비가 있고 스폰된 좀비가 최대 수보다 적을 때만 예정 1마리를 소모하고 true를 돌려준다.
    public bool TryConsumeScheduledCount()
    {
        if (ScheduledCount <= 0 || SpawnedCount >= MaxCount)
        {
            return false;
        }

        ScheduledCount--;
        return true;
    }

    // Zombie가 활성화/비활성화될 때 스스로 등록·해제한다.
    public void Register(Zombie zombie)
    {
        if (!_zombies.Contains(zombie))
        {
            _zombies.Add(zombie);
            ZombieCountChanged?.Invoke();
        }
    }

    // 좀비가 죽었을 때: 스폰된 수에서 빼고, 빈 자리를 채울 좀비를 스폰 예정에 추가한다.
    public void NotifyZombieDied(Zombie zombie)
    {
        if (_zombies.Remove(zombie))
        {
            ScheduledCount++;
            ZombieCountChanged?.Invoke();
        }
    }

    public void Unregister(Zombie zombie)
    {
        if (_zombies.Remove(zombie))
        {
            ZombieCountChanged?.Invoke();
        }
    }

    // 체력 비율 (0~1)
    public float GetHealthRatio(Zombie zombie)
    {
        return zombie.Health / zombie.MaxHealth;
    }
}