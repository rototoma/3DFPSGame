using UnityEngine;

// 좌클릭으로 총 발사
public class PlayerGunFire : MonoBehaviour
{
    private float _range = 100f;
    private float _damage = 10f;

    [SerializeField] public Transform FirePoint;

    private void Update()
    {
        // 1. 마우스 왼쪽 버튼 입력 받기
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Ray: 레이저 (시작위치, 날아갈 방향)
        // Physics.RayCast: 레이저를 발사
        // RayCastHit: 레이저가 물체와 부딪히면 호출

        // 2. Ray를 생성하고 발사될 위치와 진행 방향 설정하기
        Ray ray = new Ray(FirePoint.position, FirePoint.forward);

        // 3. Ray가 부딪힌 대상의 정보를 저장할 변수 생성하기
        RaycastHit hitInfo = new RaycastHit();

        // 4. 레이를 발사하고, 부딪힌 물체가 있다면 이펙트 표시하기
        bool isHit = Physics.Raycast(ray, out hitInfo, _range);
        if (isHit)
        {
            GameLogger.Log($"{hitInfo.collider.name}이 총알을 맞았다. 대미지: {_damage}");
        }
    }
}