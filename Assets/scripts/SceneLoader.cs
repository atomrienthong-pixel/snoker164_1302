using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sends the player through the loading screen instead of straight to a scene,
/// so there is somewhere for the progress bar to live.
/// </summary>
public static class SceneLoader
{
    public const string MenuScene = "MainMenu";
    public const string LoadingScene = "Loading";
    public const string GameScene = "SampleScene";

    /// <summary>Scene the loading screen should move on to.</summary>
    public static string NextScene { get; private set; } = GameScene;

    public static void LoadWithScreen(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[SceneLoader] No scene name given.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[SceneLoader] \"{sceneName}\" is not in Build Settings.");
            return;
        }

        NextScene = sceneName;
        Time.timeScale = 1f;

        // If the loading scene is missing, go straight there rather than
        // leaving the player stuck on a dead button.
        if (Application.CanStreamedLevelBeLoaded(LoadingScene))
            SceneManager.LoadScene(LoadingScene);
        else
            SceneManager.LoadScene(sceneName);
    }
}
