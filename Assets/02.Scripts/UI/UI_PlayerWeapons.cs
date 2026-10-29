using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerWeapons : MonoBehaviour
{
    [SerializeField] private Image _shotGunImg;
    [SerializeField] private Image _sniperImg;
    [SerializeField] private TextMeshProUGUI _bulletText;
    [SerializeField] private TextMeshProUGUI _bombText;

    public void RefreshWeapon(WeaponMode mode)
    {
        if (mode == WeaponMode.Normal)
        {
            _shotGunImg.gameObject.SetActive(true);
            _sniperImg.gameObject.SetActive(false);
        }
        else
        {
            _shotGunImg.gameObject.SetActive(false);
            _sniperImg.gameObject.SetActive(true);
        }
    }

    public void RefreshBullet(int current, int max)
    {
        _bulletText.SetText($"{current}/{max}");
    }

    public void RefreshBomb(int current, int max)
    {
        _bombText.SetText($"폭탄 {max - current}/{max}");
    }
}