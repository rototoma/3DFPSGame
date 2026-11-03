using System.Collections;
using TMPro;
using UnityEngine;

// 로그인 화면 UI 연출.
//  - 시작할 때 타이틀과 메뉴가 순서대로 나타난다.
//  - 경고 문구가 깜빡이고, 시작 버튼이 숨 쉬듯 커졌다 작아진다.
//  - 하단의 조작 팁이 일정 간격으로 바뀐다.
public class UI_LoginMenu : MonoBehaviour
{
    private const float WarningBlinkSpeed = 5f;

    [Header("등장 연출")]
    [SerializeField] private CanvasGroup _titleGroup;
    [SerializeField] private RectTransform _titleRoot;
    [SerializeField] private CanvasGroup _menuGroup;
    [SerializeField] private float _introDuration = 1f;
    [SerializeField] private float _titleSlideDistance = 80f; // 왼쪽에서 이 거리만큼 미끄러져 들어온다
    [SerializeField] private float _menuDelay = 0.6f;

    [Header("경고 문구")]
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private float _warningMinAlpha = 0.25f;

    [Header("시작 버튼")]
    [SerializeField] private RectTransform _startButton;
    [SerializeField] private float _pulseScale = 0.03f;
    [SerializeField] private float _pulseSpeed = 2.5f;

    [Header("조작 팁")]
    [SerializeField] private TMP_Text _tipText;
    [SerializeField] private CanvasGroup _tipGroup;
    [SerializeField] private float _tipFadeDuration = 0.5f;
    [SerializeField] private float _tipHoldDuration = 3.5f;
    [SerializeField, TextArea] private string[] _tips;

    private Vector2 _titleBasePosition;

    private void Awake()
    {
        _titleBasePosition = _titleRoot.anchoredPosition;
        _titleGroup.alpha = 0f;
        _menuGroup.alpha = 0f;
        _tipGroup.alpha = 0f;
    }

    private void Start()
    {
        StartCoroutine(Intro_Coroutine());
        if (_tips != null && _tips.Length > 0)
        {
            StartCoroutine(Tips_Coroutine());
        }
    }

    private void Update()
    {
        // 경고 문구 깜빡임
        float blink = Mathf.Lerp(_warningMinAlpha, 1f, (Mathf.Sin(Time.time * WarningBlinkSpeed) + 1f) * 0.5f);
        Color color = _warningText.color;
        color.a = blink;
        _warningText.color = color;

        // 시작 버튼이 숨 쉬듯 살짝 커졌다 작아진다.
        float scale = 1f + Mathf.Sin(Time.time * _pulseSpeed) * _pulseScale;
        _startButton.localScale = Vector3.one * scale;
    }

    private IEnumerator Intro_Coroutine()
    {
        // 타이틀: 왼쪽에서 미끄러지며 나타난다.
        float elapsed = 0f;
        while (elapsed < _introDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / _introDuration));
            _titleGroup.alpha = t;
            _titleRoot.anchoredPosition = _titleBasePosition + Vector2.left * (_titleSlideDistance * (1f - t));
            yield return null;
        }

        _titleGroup.alpha = 1f;
        _titleRoot.anchoredPosition = _titleBasePosition;

        // 메뉴: 잠시 뒤 페이드인
        yield return new WaitForSeconds(_menuDelay);
        yield return Fade_Coroutine(_menuGroup, 0f, 1f, _introDuration * 0.6f);
    }

    private IEnumerator Tips_Coroutine()
    {
        int index = Random.Range(0, _tips.Length);
        yield return new WaitForSeconds(_introDuration + _menuDelay);

        while (true)
        {
            _tipText.text = _tips[index];
            yield return Fade_Coroutine(_tipGroup, 0f, 1f, _tipFadeDuration);
            yield return new WaitForSeconds(_tipHoldDuration);
            yield return Fade_Coroutine(_tipGroup, 1f, 0f, _tipFadeDuration);

            index = (index + 1) % _tips.Length;
        }
    }

    private IEnumerator Fade_Coroutine(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        group.alpha = to;
    }
}
