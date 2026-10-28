using System.Collections;
using TMPro;
using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
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

    public void GameOver()
    {
        _gameState = GameState.GameOver;
        _stateText.gameObject.SetActive(true);
        _stateText.SetText("실패....");
        _screenEffect.Grad();
        _playerAnimator.SetFloat("MoveMotion", 0f); // 달리던 중이었어도 서 있는 동작으로 돌린다.
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