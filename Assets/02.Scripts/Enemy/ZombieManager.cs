using System.Collections.Generic;
using UnityEngine;

// 씬의 모든 좀비를 관리하는 싱글톤: 외부에서는 Zombie를 직접 찾지 않고 이 클래스를 거친다.
public class ZombieManager : MonoBehaviour
{
    private static ZombieManager _instance;
    public static ZombieManager Instance => _instance;

    private List<Zombie> _zombies = new List<Zombie>();

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

    // Zombie가 활성화/비활성화될 때 스스로 등록·해제한다.
    public void Register(Zombie zombie)
    {
        if (!_zombies.Contains(zombie))
        {
            _zombies.Add(zombie);
        }
    }

    public void Unregister(Zombie zombie)
    {
        _zombies.Remove(zombie);
    }

    // 체력 비율 (0~1)
    public float GetHealthRatio(Zombie zombie)
    {
        return zombie.Health / zombie.MaxHealth;
    }
}