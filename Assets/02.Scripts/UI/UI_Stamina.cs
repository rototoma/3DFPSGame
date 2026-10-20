using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stamina : MonoBehaviour
{
    [SerializeField] Slider _staminaSlider;

    public void Start()
    {
        _staminaSlider.maxValue = PlayerManager.Instance.PlayerStamina;
    }

    public void LateUpdate()
    {
        _staminaSlider.value = PlayerManager.Instance.PlayerStamina;
    }
}