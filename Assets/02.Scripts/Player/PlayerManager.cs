using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private const float PlayerMaxStamina = 10f;
    public float PlayerStamina { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PlayerStamina = PlayerMaxStamina;
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
        PlayerStamina = Mathf.Clamp(PlayerStamina + Time.deltaTime * 0.5f, 0f, PlayerMaxStamina);
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
}