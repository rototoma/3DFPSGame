using System;
using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    private Zombie _zombie;

    private void Awake()
    {
        _zombie = GetComponent<Zombie>();
    }

    private void LateUpdate()
    {
        _healthBar.value = _zombie.Health / _zombie.MaxHealth;

        // 빌보드: 체력바가 어디서든 카메라에 잘 보이도록 함
        transform.forward = Camera.main.transform.forward;
    }
}