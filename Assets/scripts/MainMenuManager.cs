using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private string gameSceneName = "SampleScene";

    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button howToButton;

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private Button closeHowToButton;

    [SerializeField]
    private GameObject howToPanel;

    [SerializeField]
    private RectTransform title;

    [SerializeField]
    private float titlePulse = 0.03f;

    [SerializeField]
    private float titleSpeed = 2f;

    private void Awake()
    {
        ShowHowTo(false);
    }

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);

        if (howToButton != null)
            howToButton.onClick.AddListener(OpenHowTo);

        if (closeHowToButton != null)
            closeHowToButton.onClick.AddListener(CloseHowTo);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (title == null)
            return;

        float scale = 1f + Mathf.Sin(Time.time * titleSpeed) * titlePulse;
        title.localScale = new Vector3(scale, scale, 1f);
    }

    public void PlayGame()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButton();

        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenHowTo()
    {
        ShowHowTo(true);
    }

    public void CloseHowTo()
    {
        ShowHowTo(false);
    }

    public void ShowHowTo(bool show)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButton();

        if (howToPanel != null)
            howToPanel.SetActive(show);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
