using TMPro;
using UnityEngine;

// 옵저버: ZombieManager의 ZombieCountChanged를 구독해 "Zombies 현재/최대"를 표시한다.
public class UI_ZombieCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        ZombieManager.Instance.ZombieCountChanged += RefreshUI;
        RefreshUI(); // 구독 전에 이미 바뀐 값을 반영한다.
    }

    private void OnDestroy()
    {
        if (ZombieManager.Instance != null)
        {
            ZombieManager.Instance.ZombieCountChanged -= RefreshUI;
        }
    }

    private void RefreshUI()
    {
        _text.SetText($"Zombies {ZombieManager.Instance.SpawnedCount}/{ZombieManager.Instance.MaxCount}");
    }
}
