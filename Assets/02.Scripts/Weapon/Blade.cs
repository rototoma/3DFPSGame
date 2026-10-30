using UnityEngine;

public class Blade : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _range = 1f;
    [SerializeField] private Collider _collider;
    [SerializeField] private ParticleSystem _hitEffectPrefab;


    private void OnTriggerEnter(Collider other)
    {
        if (PlayerManager.Instance.PlayerMeleeHitMode != MeleeHitMode.Collider) return;

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


    public void ActivateController()
    {
        if (PlayerManager.Instance.PlayerMeleeHitMode == MeleeHitMode.Collider)
        {
            _collider.enabled = true;
        }
        else
        {
            FanAttack();
        }
    }

    public void DeactivateController() => _collider.enabled = false;


    public void FanAttack()
    {
        if (PlayerManager.Instance.PlayerMeleeHitMode != MeleeHitMode.Fan) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _range);

        foreach (Collider col in colliders)
        {
            var closest = col.ClosestPoint(transform.position);
            DamageInfo damageInfo =
                new DamageInfo(_damage, closest, (gameObject.transform.position - closest).normalized);
            col.GetComponent<IDamageable>()?.TakeDamage(damageInfo);
        }
    }
}