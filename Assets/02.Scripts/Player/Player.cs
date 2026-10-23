using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject _bloodSprayPrefab;

    public void TakeDamage(DamageInfo damageInfo)
    {
        GameObject bloodSpray = Instantiate(_bloodSprayPrefab, gameObject.transform);
        bloodSpray.transform.position = damageInfo.HitPoint;
        bloodSpray.transform.forward = damageInfo.HitNormal;

        PlayerManager.Instance.DecreaseHealth(damageInfo.Value);
    }
}