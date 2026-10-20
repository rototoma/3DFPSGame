using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public float PlayerStamina = 10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
        PlayerStamina += Time.deltaTime * 0.5f;
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