using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private const float PlayerMaxStamina = 10f;
    public float PlayerStamina { get; private set; }
    private const float PlayerMaxHealth = 100f;
    public float PlayerPublicMaxHealth => PlayerMaxHealth;
    public float PlayerHealth { get; private set; }

    public WeaponMode PlayerWeaponMode = WeaponMode.Normal;
    public MeleeHitMode PlayerMeleeHitMode = MeleeHitMode.Collider;

    [SerializeField] private UI_ScreenEffect _hitScreen;

    // 옵저버: 무기가 교체되면 UI 등 구독자에게 알린다.
    public event Action<WeaponMode> WeaponModeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PlayerStamina = PlayerMaxStamina;
        PlayerHealth = PlayerMaxHealth;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        // Game Play상태가 아니라면 연산x
        if (GameManager.Instance.GameState != GameState.Playing) return;
        PlayerStamina = Mathf.Clamp(PlayerStamina + Time.deltaTime * 0.5f, 0f, PlayerMaxStamina);
    }

    public void DecreaseHealth(float amount)
    {
        if (PlayerHealth <= 0)
        {
            GameManager.Instance.GameOver();
            return;
        }

        PlayerHealth -= amount;
        _hitScreen.Fade();
    }


    public bool UseStamina(float amount)
    {
        if (PlayerStamina <= 0)
        {
            return false;
        }

        PlayerStamina -= amount;
        return true;
    }

    public void ChangeWeaponMode(WeaponMode mode)
    {
        PlayerWeaponMode = mode;
        WeaponModeChanged?.Invoke(mode);
    }
}