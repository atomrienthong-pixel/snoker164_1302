using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private Button openButton;

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private Text musicValueText;

    [SerializeField]
    private Text sfxValueText;

    [SerializeField]
    private bool pauseWhileOpen;

    private bool open;
    private float savedScale = 1f;

    private void Awake()
    {
        Show(false);
    }

    private void Start()
    {
        if (openButton != null)
            openButton.onClick.AddListener(Open);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        float music = AudioManager.instance != null ? AudioManager.instance.MusicVolume : 0.7f;
        float sfx = AudioManager.instance != null ? AudioManager.instance.SfxVolume : 1f;

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(music);
            musicSlider.onValueChanged.AddListener(SetMusic);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(sfx);
            sfxSlider.onValueChanged.AddListener(SetSfx);
        }

        ShowMusicValue(music);
        ShowSfxValue(sfx);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Toggle();
    }

    public void SetMusic(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.MusicVolume = value;

        ShowMusicValue(value);
    }

    public void SetSfx(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SfxVolume = value;

        ShowSfxValue(value);
    }

    private void ShowMusicValue(float value)
    {
        if (musicValueText != null)
            musicValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void ShowSfxValue(float value)
    {
        if (sfxValueText != null)
            sfxValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void Toggle()
    {
        Show(!open);
    }

    public void Open()
    {
        Show(true);
    }

    public void Close()
    {
        Show(false);
    }

    private void Show(bool value)
    {
        open = value;

        if (panel != null)
            panel.SetActive(value);

        if (pauseWhileOpen)
        {
            if (value)
            {
                savedScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = savedScale;
            }
        }

        if (value && AudioManager.instance != null)
            AudioManager.instance.PlayButton();
    }
}
