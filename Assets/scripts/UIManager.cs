using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text turnText;

    [SerializeField]
    private Text messageText;

    [SerializeField]
    private RectTransform powerFill;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private Text gameOverText;

    [SerializeField]
    private Button restartButton;

    private void Awake()
    {
        instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        ShowPower(0f);
    }

    private void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
    }

    public void ShowScore(int score1, int score2)
    {
        if (scoreText != null)
            scoreText.text = "P1 " + score1 + "   |   P2 " + score2;
    }

    public void ShowTurn(int turn, bool needRed)
    {
        if (turnText != null)
            turnText.text = "Player " + (turn + 1) + " - hit " + (needRed ? "RED" : "COLOR");
    }

    public void ShowMessage(string text)
    {
        if (messageText != null)
            messageText.text = text;
    }

    public void ShowPower(float amount)
    {
        if (powerFill != null)
            powerFill.localScale = new Vector3(Mathf.Clamp01(amount), 1f, 1f);
    }

    public void ShowGameOver(string text)
    {
        if (gameOverText != null)
            gameOverText.text = text;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneLoader.LoadWithScreen(SceneManager.GetActiveScene().name);
    }
}
