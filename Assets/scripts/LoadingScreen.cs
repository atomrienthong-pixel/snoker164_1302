using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Loads the next scene in the background and drives a slider with the
/// progress. The bar is eased rather than snapped so a fast load still reads as
/// a bar filling up instead of a flicker.
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private Text percentText;

    [SerializeField]
    [Tooltip("Shortest time the screen stays up, so it does not just blink past.")]
    private float minimumDuration = 1.2f;

    [SerializeField]
    [Tooltip("How fast the bar catches up to the real figure.")]
    private float barSpeed = 1.5f;

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = 0f;
        }

        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        string target = SceneLoader.NextScene;

        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            Debug.LogError($"[LoadingScreen] \"{target}\" is not in Build Settings.");
            yield break;
        }

        var operation = SceneManager.LoadSceneAsync(target);

        // Hold the finished scene back until the bar has actually filled.
        operation.allowSceneActivation = false;

        float shown = 0f;
        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.unscaledDeltaTime;

            // Unity stops at 0.9 while activation is held off, so stretch that
            // range back out to a full bar.
            float real = Mathf.Clamp01(operation.progress / 0.9f);
            float timeGate = minimumDuration > 0f
                ? Mathf.Clamp01(elapsed / minimumDuration)
                : 1f;
            float target01 = Mathf.Min(real, timeGate);

            shown = Mathf.MoveTowards(shown, target01, barSpeed * Time.unscaledDeltaTime);
            Report(shown);

            if (shown >= 0.999f && real >= 1f && elapsed >= minimumDuration)
                break;

            yield return null;
        }

        Report(1f);
        yield return null;

        operation.allowSceneActivation = true;
    }

    private void Report(float value)
    {
        if (progressBar != null)
            progressBar.value = value;

        if (percentText != null)
            percentText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
