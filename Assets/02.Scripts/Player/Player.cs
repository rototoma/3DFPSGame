using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject _bloodSprayPrefab;
    [SerializeField] private Animator _animator;
    private int _injuredLayerIndex;

    public void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _injuredLayerIndex = _animator.GetLayerIndex("Injured Layer");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerManager.Instance.ChangeWeaponMode(WeaponMode.Normal);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerManager.Instance.ChangeWeaponMode(WeaponMode.Sniper);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerManager.Instance.ChangeWeaponMode(WeaponMode.Melee);
        }
    }

    public event Action OnDamaged = null;

    public void TakeDamage(DamageInfo damageInfo)
    {
        GameObject bloodSpray = Instantiate(_bloodSprayPrefab, gameObject.transform);
        bloodSpray.transform.position = damageInfo.HitPoint;
        bloodSpray.transform.forward = damageInfo.HitNormal;

        PlayerManager.Instance.DecreaseHealth(damageInfo.Value);
        float maxHealth = PlayerManager.Instance.PlayerPublicMaxHealth;
        float currentHealth = PlayerManager.Instance.PlayerHealth;

        float injured = Mathf.InverseLerp(0.5f, 0f, currentHealth / maxHealth);
        _animator.SetLayerWeight(_injuredLayerIndex, injured);

        OnDamaged?.Invoke();
    }
}