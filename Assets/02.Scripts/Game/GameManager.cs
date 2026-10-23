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
        _ScreenEffect.Grad();
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