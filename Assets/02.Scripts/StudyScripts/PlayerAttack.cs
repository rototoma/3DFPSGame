using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float _meleeCooldown;
    [SerializeField] private float _swordCooldown;

    private void Start()
    {
    }

    private void Update()
    {
        // 평타 좌클릭
        bool isMelee = Input.GetMouseButton(0);
        // 스킬 우클릭
        bool isSkill = Input.GetMouseButton(1);
    }
}