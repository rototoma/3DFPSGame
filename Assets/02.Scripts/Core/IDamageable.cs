using UnityEngine;

// 대미지를 받을 수 있게 되는 모든 오브젝트는 이 인터페이스를 상속하도록 약속한다.
public interface IDamageable
{
    public void TakeDamage(DamageInfo damageInfo);
}