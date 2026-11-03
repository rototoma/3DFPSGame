using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// LoginScene을 관리하는 메인 스크립트.
// 이 씬에서 플레이어는 이동하지 않고 애니메이션만 재생한다.
public class LoginSceneManager : MonoBehaviour
{
    [Header("플레이어 연출")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private string _startAnimationState; // 씬이 시작될 때 재생할 애니메이션 상태 이름 (비워 두면 컨트롤러의 기본 상태)

    [Header("두리번거리기 (가만히 서 있는 어색함을 줄이는 연출)")]
    [SerializeField] private Transform _lookAroundTarget;       // 방향을 돌릴 플레이어 (루트)
    [SerializeField] private float _lookAngle = 10f;            // 좌우로 돌아볼 각도
    [SerializeField] private float _minLookInterval = 1f;       // 다음 두리번거림까지 최소 대기 시간
    [SerializeField] private float _maxLookInterval = 3f;       // 다음 두리번거림까지 최대 대기 시간
    [SerializeField] private float _turnDuration = 0.5f;        // 한 방향으로 돌거나 제자리로 돌아오는 데 걸리는 시간
    [SerializeField] private float _holdDuration = 0.4f;        // 돌아본 상태로 머무는 시간

    [Header("씬 전환")]
    [SerializeField] private string _gameSceneName = "GameScene";

    private void Awake()
    {
        // 다른 씬(일시정지 등)에서 넘어와도 시간이 멈춰 있지 않도록 되돌린다.
        Time.timeScale = 1f;

        // 로그인 화면에서는 마우스로 UI를 조작한다.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 애니메이션의 루트 모션으로 캐릭터가 움직이지 않게 한다.
        if (_playerAnimator != null)
        {
            _playerAnimator.applyRootMotion = false;
        }
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(_startAnimationState))
        {
            PlayAnimation(_startAnimationState);
        }

        if (_lookAroundTarget != null)
        {
            StartCoroutine(LookAround_Coroutine());
        }
    }

    // 1~3초 간격(랜덤)으로 한쪽(+각도)을 돌아봤다가 돌아오고, 다음에는 반대쪽(-각도)을 돌아봤다가 돌아온다.
    private IEnumerator LookAround_Coroutine()
    {
        Quaternion baseRotation = _lookAroundTarget.rotation;
        float direction = 1f;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_minLookInterval, _maxLookInterval));

            Quaternion lookRotation = baseRotation * Quaternion.Euler(0f, _lookAngle * direction, 0f);
            yield return Rotate_Coroutine(baseRotation, lookRotation);
            yield return new WaitForSeconds(_holdDuration);
            yield return Rotate_Coroutine(lookRotation, baseRotation);

            direction = -direction;
        }
    }

    private IEnumerator Rotate_Coroutine(Quaternion from, Quaternion to)
    {
        float elapsed = 0f;
        while (elapsed < _turnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / _turnDuration)); // 시작과 끝이 부드럽게
            _lookAroundTarget.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        _lookAroundTarget.rotation = to;
    }

    // 플레이어 애니메이션 상태를 바로 재생한다.
    public void PlayAnimation(string stateName)
    {
        if (_playerAnimator == null) return;
        _playerAnimator.Play(stateName);
    }

    // 플레이어 애니메이터의 트리거를 발동한다.
    public void TriggerAnimation(string triggerName)
    {
        if (_playerAnimator == null) return;
        _playerAnimator.SetTrigger(triggerName);
    }

    // 게임 시작 버튼 (UI의 OnClick에 연결)
    public void OnClickStartGame()
    {
        SceneManager.LoadScene(_gameSceneName);
    }

    // 게임 종료 버튼 (UI의 OnClick에 연결)
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
