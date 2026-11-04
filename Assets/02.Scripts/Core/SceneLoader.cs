using UnityEngine;
using UnityEngine.SceneManagement;

// 씬을 바로 불러오지 않고 LoadingScene을 거쳐서 불러온다.
// 어떤 씬으로 갈지는 여기에 적어 두고, LoadingScene의 UI_LoadingScreen이 읽어 간다.
public static class SceneLoader
{
    public const string LoadingSceneName = "LoadingScene";

    // 로딩이 끝나면 넘어갈 씬 이름
    public static string TargetSceneName { get; private set; }

    // 로딩 화면을 거쳐 targetSceneName 씬으로 간다.
    public static void Load(string targetSceneName)
    {
        TargetSceneName = targetSceneName;
        SceneManager.LoadScene(LoadingSceneName);
    }
}