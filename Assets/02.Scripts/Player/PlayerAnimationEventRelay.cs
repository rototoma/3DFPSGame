using UnityEngine;

// Animator가 붙은 Tourist_male에 부착하여, Animation Event를 자식(Blade)의 같은 이름 함수로 전달한다.
public class PlayerAnimationEventRelay : MonoBehaviour
{
    private Blade _blade;

    private void Awake()
    {
        // 무기가 비활성 상태일 수 있으므로 비활성 오브젝트도 검색한다.
        _blade = GetComponentInChildren<Blade>(true);
    }

    // Animation Event 함수명: ActivateController
    public void ActivateController() => _blade.ActivateController();

    // Animation Event 함수명: DeactivateController
    public void DeactivateController() => _blade.DeactivateController();
}
