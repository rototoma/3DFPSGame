using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public void TakeDamage(DamageInfo damageInfo)
    {
        PlayerManager.Instance.DecreaseHealth(damageInfo.Value);
    }
}