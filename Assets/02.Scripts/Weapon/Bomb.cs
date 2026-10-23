using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;
    [SerializeField] private float _explosionRange = 4.5f;
    [SerializeField] private float _damage = 100f;

    private PlayerBombFire _parentScript;

    public void Init(PlayerBombFire parent)
    {
        _parentScript = parent;
    }

    private void ApplyAreaDamage()
    {
        // position, radius, layerMask
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3, LayerMask.GetMask("Drum"));

        foreach (Collider col in colliders)
        {
            var closest = col.ClosestPoint(transform.position);
            DamageInfo damageInfo =
                new DamageInfo(_damage, closest, (gameObject.transform.position - closest).normalized);
            col.GetComponent<IDamageable>()?.TakeDamage(damageInfo);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem effect =
            Lean.Pool.LeanPool.Spawn(_explosionEffectPrefab, transform.localPosition, Quaternion.identity);
        effect.Play();
        ApplyAreaDamage();

        if (_parentScript != null)
        {
            _parentScript.BombDestroyed();
        }

        Lean.Pool.LeanPool.Despawn(gameObject);
    }
}