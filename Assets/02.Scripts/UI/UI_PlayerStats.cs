using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] Slider _staminaSlider;
    [SerializeField] Slider _healthSlider;

    public void Start()
    {
        _staminaSlider.maxValue = PlayerManager.Instance.PlayerStamina;
        _healthSlider.maxValue = PlayerManager.Instance.PlayerHealth;
    }

    public void LateUpdate()
    {
        _staminaSlider.value = PlayerManager.Instance.PlayerStamina;
        _healthSlider.value = PlayerManager.Instance.PlayerHealth;
    }
}