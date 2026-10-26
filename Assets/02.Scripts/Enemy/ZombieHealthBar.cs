using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Zombie _zombie;

    private void OnEnable()
    {
        _zombie.HealthChanged += RefreshHealthBar;
        RefreshHealthBar(); // Zombie의 초기화 순서와 상관없이 현재 체력을 바로 반영한다.
    }

    private void OnDisable()
    {
        _zombie.HealthChanged -= RefreshHealthBar;
    }

    private void LateUpdate()
    {
        // 빌보드: 체력바가 어디서든 카메라에 잘 보이도록 함
        _healthBar.transform.forward = Camera.main.transform.forward;
    }

    private void RefreshHealthBar()
    {
        _healthBar.value = ZombieManager.Instance.GetHealthRatio(_zombie);
    }
}