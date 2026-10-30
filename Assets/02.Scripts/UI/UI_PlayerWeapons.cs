using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 옵저버: 무기 교체, 총알 수, 폭탄 수 변경 이벤트를 구독해 UI를 갱신한다.
public class UI_PlayerWeapons : MonoBehaviour
{
    [SerializeField] private Image _shotGunImg;
    [SerializeField] private Image _sniperImg;
    [SerializeField] private Image _meleeImg;

    [SerializeField] private TextMeshProUGUI _bulletText;
    [SerializeField] private TextMeshProUGUI _bombText;
    [SerializeField] private PlayerGunFire _gunFire;
    [SerializeField] private PlayerBombFire _bombFire;

    private void Start()
    {
        PlayerManager.Instance.WeaponModeChanged += RefreshWeapon;
        _gunFire.BulletCountChanged += RefreshBullet;
        _bombFire.BombCountChanged += RefreshBomb;

        // 구독 전에 이미 정해진 값을 반영한다.
        RefreshWeapon(PlayerManager.Instance.PlayerWeaponMode);
        RefreshBullet(_gunFire.BulletCount, _gunFire.MaxBulletCount);
        RefreshBomb(_bombFire.CurrentBomb, _bombFire.MaxBombs);
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.WeaponModeChanged -= RefreshWeapon;
        }

        if (_gunFire != null)
        {
            _gunFire.BulletCountChanged -= RefreshBullet;
        }

        if (_bombFire != null)
        {
            _bombFire.BombCountChanged -= RefreshBomb;
        }
    }

    private void RefreshWeapon(WeaponMode mode)
    {
        if (mode == WeaponMode.Normal)
        {
            _shotGunImg.gameObject.SetActive(true);
            _sniperImg.gameObject.SetActive(false);
        }
        else if (mode == WeaponMode.Sniper)
        {
            _shotGunImg.gameObject.SetActive(false);
            _sniperImg.gameObject.SetActive(true);
        }
        else
        {
            _shotGunImg.gameObject.SetActive(false);
            _sniperImg.gameObject.SetActive(true);
        }
    }

    private void RefreshBullet(int current, int max)
    {
        _bulletText.SetText($"{current}/{max}");
    }

    private void RefreshBomb(int current, int max)
    {
        _bombText.SetText($"폭탄 {max - current}/{max}");
    }
}