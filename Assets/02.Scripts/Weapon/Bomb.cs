using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;

    public PlayerBombFire ParentScript;

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem effect =
            Lean.Pool.LeanPool.Spawn(_explosionEffectPrefab, transform.localPosition, Quaternion.identity);
        effect.Play();

        //Todo: 풀링 적용
        Lean.Pool.LeanPool.Despawn(gameObject);
        ParentScript.BombDestroyed();
    }
}