using UnityEngine;
using UnityEngine.SceneManagement;

// 일시정지 창의 버튼 기능을 담당한다. 각 버튼의 OnClick에 연결해서 사용한다.
public class UI_PauseMenu : MonoBehaviour
{
    // 계속하기
    public void OnClickResume()
    {
        GameManager.Instance.Resume();
    }

    // 다시 시작
    public void OnClickRestart()
    {
        Time.timeScale = 1f; // 일시정지로 멈춰둔 시간을 되돌려야 다시 시작한 씬이 멈추지 않는다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 게임 종료
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
