using System;
using System.Collections;
using TMPro;
using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
    Pause,
    GameOver,
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

    private GameState _gameState = GameState.Ready;
    public GameState GameState => _gameState;

    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private UI_ScreenEffect _screenEffect;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private float _gameOverPanelDelay = 1.5f; // 화면이 붉게 덮이는 연출을 잠시 보여준 뒤 팝업을 띄운다.

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        StartCoroutine(StartSequence_Coroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_gameState == GameState.Playing)
            {
                _gameState = GameState.Pause;
                Time.timeScale = 0;
                _pausePanel.SetActive(true);
                CursorManager.Instance.UnlockCursor();
            }
            else if (_gameState == GameState.Pause)
            {
                Resume();
            }
        }
    }

    public void Resume()
    {
        _gameState = GameState.Playing;
        Time.timeScale = 1;
        _pausePanel.SetActive(false);
        CursorManager.Instance.LockCursor();
    }

    public void GameOver()
    {
        _gameState = GameState.GameOver;
        _stateText.gameObject.SetActive(true);
        _stateText.SetText("실패....");
        _screenEffect.Grad();
        _playerAnimator.SetFloat("MoveMotion", 0f); // 달리던 중이었어도 서 있는 동작으로 돌린다.
        StartCoroutine(ShowGameOverPanel_Coroutine());
    }

    private IEnumerator ShowGameOverPanel_Coroutine()
    {
        yield return new WaitForSeconds(_gameOverPanelDelay);
        _stateText.gameObject.SetActive(false); // 팝업 뒤로 "실패...." 글자가 비치지 않게 한다.
        _gameOverPanel.SetActive(true);
        CursorManager.Instance.UnlockCursor();
    }

    private IEnumerator StartSequence_Coroutine()
    {
        _stateText.SetText("준비..");
        yield return new WaitForSeconds(2.0f);
        _gameState = GameState.Playing;
        _stateText.SetText("시작!");
        yield return new WaitForSeconds(1.0f);
        _stateText.gameObject.SetActive(false);
    }
}