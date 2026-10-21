using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;

    private PlayerBombFire _parentScript;

    public void Init(PlayerBombFire parent)
    {
        _parentScript = parent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem effect =
            Lean.Pool.LeanPool.Spawn(_explosionEffectPrefab, transform.localPosition, Quaternion.identity);
        effect.Play();

        if (_parentScript != null)
        {
            _parentScript.BombDestroyed();
        }

        Lean.Pool.LeanPool.Despawn(gameObject);
    }
}