using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerZoom : MonoBehaviour
{
    [SerializeField] private GameObject _sniperZoom;
    [SerializeField] private float _sniperPov = 15f;
    [SerializeField] private float _normalPov = 60f;

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing) return;

        bool isZooming = PlayerManager.Instance.PlayerWeaponMode == WeaponMode.Sniper;
        _sniperZoom.SetActive(isZooming);
        Camera.main.fieldOfView = isZooming ? _sniperPov : _normalPov;
    }
}