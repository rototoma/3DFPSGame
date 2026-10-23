using System;
using UnityEngine;

public class Box : MonoBehaviour, IDamageable
{
    [Header("스탯")]
    [SerializeField] private float _health = 30f;

    [SerializeField] private float _destroyDelay = 3f;
    [SerializeField] private float _launchPower = 3f;

    [SerializeField] private Rigidbody[] _breakOutObj = new Rigidbody[8];

    public void Awake()
    {
        foreach (var body in _breakOutObj)
        {
            body.gameObject.SetActive(false);
        }
    }

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
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        foreach (var body in _breakOutObj)
        {
            body.gameObject.SetActive(true);
            var forceDirection = (transform.position - body.transform.localPosition).normalized;
            body.AddForce(forceDirection * _launchPower, ForceMode.Impulse);
            body.AddTorque(UnityEngine.Random.insideUnitSphere, ForceMode.Impulse);
        }

        Destroy(gameObject, _destroyDelay);
    }
}