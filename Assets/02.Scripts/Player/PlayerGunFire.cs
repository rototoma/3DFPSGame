using UnityEngine;

// 좌클릭으로 총 발사
public class PlayerGunFire : MonoBehaviour
{
    private float _range = 100f;
    private float _damage = 10f;
    private float _fireInterval = 0.1f;
    private float _fireCoolDown = 0;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _originPoint;
    [SerializeField] private ParticleSystem _hitEffectPrefab;

    private void Update()
    {
        // 1. 마우스 왼쪽 버튼 입력 받기
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }

        _fireCoolDown -= Time.deltaTime;
    }

    private void Shoot()
    {
        if (_fireCoolDown > 0) return;

        _fireCoolDown = _fireInterval;

        // Ray: 레이저 (시작위치, 날아갈 방향)
        // Physics.RayCast: 레이저를 발사
        // RayCastHit: 레이저가 물체와 부딪히면 호출

        // 2. Ray를 생성하고 발사될 위치와 진행 방향 설정하기
        Transform cameraTr = Camera.main.transform;
        Ray ray = new Ray(cameraTr.position, cameraTr.forward);

        // 3. Ray가 부딪힌 대상의 정보를 저장할 변수 생성하기
        RaycastHit hitInfo = new RaycastHit();

        // 4. 레이를 발사하고, 부딪힌 물체가 있다면 이펙트 표시하기
        bool isHit = Physics.Raycast(ray, out hitInfo, _range);
        if (isHit)
        {
            ParticleSystem effect = Instantiate(_hitEffectPrefab);
            effect.transform.position = hitInfo.point;
            effect.transform.forward = hitInfo.normal;
            effect.Play();
        }
    }
}