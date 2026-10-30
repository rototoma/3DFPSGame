using UnityEngine;

public class Blade : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private Collider _collider;
    [SerializeField] private ParticleSystem _hitEffectPrefab;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("attack");

        var closest = other.GetComponent<Collider>().ClosestPoint(transform.position);
        DamageInfo damageInfo =
            new DamageInfo(_damage, closest, (gameObject.transform.position - closest).normalized);
        other.GetComponent<IDamageable>()?.TakeDamage(damageInfo);

        ParticleSystem effect = Lean.Pool.LeanPool.Spawn(_hitEffectPrefab);
        effect.transform.position = damageInfo.HitPoint;
        effect.transform.forward = damageInfo.HitNormal;
        effect.Play();
    }


    public void ActivateController() => _collider.enabled = true;
    public void DeactivateController() => _collider.enabled = false;
}