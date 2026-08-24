using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioClip musicClip;

    [SerializeField]
    private AudioClip buttonClip;

    [SerializeField]
    private AudioClip cueHitClip;

    [SerializeField]
    private AudioClip ballHitClip;

    [SerializeField]
    private AudioClip potClip;

    [SerializeField]
    private AudioClip foulClip;

    [SerializeField]
    private AudioClip winClip;

    private float musicVolume = 0.7f;
    private float sfxVolume = 1f;

    public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            musicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);

            if (musicSource != null)
                musicSource.volume = musicVolume;
        }
    }

    public float SfxVolume
    {
        get { return sfxVolume; }
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        }
    }

    private void Awake()
    {
        instance = this;
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
    }

    private void Start()
    {
        PlayMusic(musicClip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayButton()
    {
        PlaySfx(buttonClip);
    }

    public void PlayCueHit()
    {
        PlaySfx(cueHitClip);
    }

    public void PlayBallHit()
    {
        PlaySfx(ballHitClip);
    }

    public void PlayPot()
    {
        PlaySfx(potClip);
    }

    public void PlayFoul()
    {
        PlaySfx(foulClip);
    }

    public void PlayWin()
    {
        PlaySfx(winClip);
    }
}
