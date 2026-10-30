using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    private Player _player;
    private Animator _animator;
    private int _meleeLayerIndex;

    [SerializeField] private Collider _collider;


    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponentInChildren<Animator>();
        _meleeLayerIndex = _animator.GetLayerIndex("Melee Layer");
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing) return;
        if (PlayerManager.Instance.PlayerWeaponMode != WeaponMode.Melee) return;

        if (Input.GetButton("Fire1"))
        {
            Attack();
            _animator.SetBool("Slash", true);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            _animator.SetBool("Slash", false);
        }
    }


    public void Attack()
    {
    }
}