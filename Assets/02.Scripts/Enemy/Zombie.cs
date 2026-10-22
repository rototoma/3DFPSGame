using UnityEngine;

public enum ZombieState
{
    Idle,
    Trace,
    Comeback,
    Attack,
    Hit,
    Death,
}

public class Zombie : MonoBehaviour
{
    // 좀비 AI를 FSM을 활용하여 구현
    private ZombieState _zombieState = ZombieState.Idle;

    [Header("스탯")]
    [SerializeField] private float _detectDistance = 8;
    [SerializeField] private float _moveSpeed = 5;

    private CharacterController _controller;
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (_zombieState == ZombieState.Idle)
        {
            Idle();
        }
        else if (_zombieState == ZombieState.Trace)
        {
            Trace();
        }

        {
        }
    }

    private void Idle()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= _detectDistance)
        {
            Debug.Log($"{_zombieState} -> trace");
            _zombieState = ZombieState.Trace;
        }
    }

    private void Trace()
    {
        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        transform.forward = direction;
        _controller.SimpleMove(direction * _moveSpeed);
    }
}