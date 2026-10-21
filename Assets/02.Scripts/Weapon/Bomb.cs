using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;

    public PlayerBombFire ParentScript;

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem effect = Instantiate(_explosionEffectPrefab, transform.localPosition, Quaternion.identity);
        effect.Play();

        //Todo: 풀링 적용
        Destroy(gameObject);
        ParentScript.BombDestroyed();
    }
}