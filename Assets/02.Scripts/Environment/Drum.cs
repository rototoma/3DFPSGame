using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Drum : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;

    [Header("스탯")]
    [SerializeField] private float _health = 30f;
    [SerializeField] private float _launchPower = 20f;
    [SerializeField] private float _spinPower = 5f;
    [SerializeField] private float _destroyDelay = 3f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Damage(float damage)
    {
        if (_health <= 0f) return;

        _health -= damage;

        if (_health <= 0)
        {
            Explode();
        }
    }

    public void Explode()
    {
        ParticleSystem effect = Lean.Pool.LeanPool.Spawn(_explosionEffectPrefab);
        effect.transform.position = transform.position;

        _rigidbody.AddForce(Vector3.up * _launchPower, ForceMode.Impulse);
        _rigidbody.AddTorque(Random.insideUnitSphere * _spinPower, ForceMode.Impulse);

        Destroy(gameObject, _destroyDelay);
    }
}