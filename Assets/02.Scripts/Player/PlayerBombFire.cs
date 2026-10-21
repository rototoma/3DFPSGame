using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    private float _fireInterval = 0.1f;
    private float _fireCoolDown = 0;
    private float _throwPower = 20f;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private Bomb _bombPrefab;

    private void Update()
    {
        // 1. 마우스 오른쪽 버튼 입력 받기
        if (Input.GetButtonDown("Fire2"))
        {
            Shoot();
        }

        _fireCoolDown -= Time.deltaTime;
    }

    private void Shoot()
    {
        Bomb bomb = Instantiate(_bombPrefab, _firePoint.position, Quaternion.identity);

        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
        rigidbody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);
    }
}