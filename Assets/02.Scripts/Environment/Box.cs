using UnityEngine;

public class Box : MonoBehaviour, IDamageable
{
    [Header("스탯")]
    [SerializeField] private float _health = 30f;

    [SerializeField] private float _destroyDelay = 1.5f;


    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_health <= 0f) return;

        _health -= damageInfo.Value;

        if (_health <= 0)
        {
            Break();
        }
    }

    public void Break()
    {
        Destroy(gameObject, _destroyDelay);
    }
}