using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 로딩 화면. SceneLoader가 정해 둔 씬을 비동기로 불러오면서 진행 정도를 슬라이더로 보여 준다.
public class UI_LoadingScreen : MonoBehaviour
{
    // LoadSceneAsync는 allowSceneActivation이 false면 progress가 0.9에서 멈춘다.
    private const float AsyncProgressLimit = 0.9f;

    [Header("UI")]
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TextMeshProUGUI _progressText; // 예: "73%"

    [Header("연출")]
    [SerializeField] private float _minLoadingTime = 1.5f; // 로딩이 너무 빨라도 최소한 이만큼은 보여 준다
    [SerializeField] private float _fillSpeed = 1f; // 슬라이더가 초당 차오르는 양 (1 = 1초에 100%)

    private void Start()
    {
        Time.timeScale = 1f;

        // 로딩 중에도 마우스 커서가 보이도록 한다.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetProgress(0f);
        StartCoroutine(Load_Coroutine());
    }

    private IEnumerator Load_Coroutine()
    {
        Debug.LogError("로딩씬");
        string targetSceneName = SceneLoader.TargetSceneName;
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("[UI_LoadingScreen] 불러올 씬이 정해지지 않았습니다. SceneLoader.Load()로 들어와야 합니다.");
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        operation.allowSceneActivation = false; // 씬 로딩이 다 되어도 즉시 로드하지 않음

        float elapsed = 0f;
        float displayed = 0f;

        while (displayed < 0.9f)
        {
            elapsed += Time.unscaledDeltaTime;

            // 실제 로딩 정도와 최소 로딩 시간 중 느린 쪽에 맞춰, 슬라이더가 부드럽게 따라가게 한다.
            float realProgress = Mathf.Clamp01(operation.progress / AsyncProgressLimit);
            float timeProgress = Mathf.Clamp01(elapsed / _minLoadingTime);
            float goal = Mathf.Min(realProgress, timeProgress);

            displayed = Mathf.MoveTowards(displayed, goal, _fillSpeed * Time.unscaledDeltaTime);
            Debug.LogError(displayed);
            SetProgress(displayed);
            yield return null;
        }

        operation.allowSceneActivation = true;
        Debug.LogError("게임씬");
    }

    private void SetProgress(float value)
    {
        _progressSlider.value = value;

        if (_progressText != null)
        {
            _progressText.text = $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }
}