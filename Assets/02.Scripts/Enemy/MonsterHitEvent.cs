using System;
using UnityEngine;

public class MonsterHitEvent : MonoBehaviour
{
    [SerializeField] private Zombie _zombie;

    private void Start()
    {
        _zombie = GetComponentInParent<Zombie>();
    }

    public void ZombieHitEvent()
    {
        _zombie.AttackAction();
    }
}