using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider sfxSlider;
    public Slider bgmSlider;
    public Button sfxButton;
    public Button bgmButton;

    [Header("Slash Overlays")]
    public GameObject sfxSlash;
    public GameObject bgmSlash;

    private bool sfxMuted = false;
    private bool bgmMuted = false;
    private float lastSFXVolume = 1f;
    private float lastBGMVolume = 1f;

    private AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.Instance;

        sfxButton.onClick.AddListener(ToggleSFX);
        bgmButton.onClick.AddListener(ToggleBGM);

        sfxSlider.value = audioManager != null ? audioManager.sfxSource.volume : 1f;
        bgmSlider.value = audioManager != null ? audioManager.musicSource.volume : 1f;

        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);

        // Khôi phục trạng thái khi mở lại menu
        sfxMuted = PlayerPrefs.GetInt("SFX_Muted", 0) == 1;
        bgmMuted = PlayerPrefs.GetInt("BGM_Muted", 0) == 1;

        float sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        float bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);

        audioManager.sfxSource.volume = sfxMuted ? 0 : sfxVolume;
        audioManager.musicSource.volume = bgmMuted ? 0 : bgmVolume;

        sfxSlider.value = audioManager.sfxSource.volume;
        bgmSlider.value = audioManager.musicSource.volume;

        sfxSlash.SetActive(sfxMuted);
        bgmSlash.SetActive(bgmMuted);
    }

    void SetSFXVolume(float value)
    {
        if (!sfxMuted && audioManager != null)
        {
            audioManager.sfxSource.volume = value;
            lastSFXVolume = value;
        }
    }

    void SetBGMVolume(float value)
    {
        if (!bgmMuted && audioManager != null)
        {
            audioManager.musicSource.volume = value;
            lastBGMVolume = value;
        }
    }

    void ToggleSFX()
    {
        if (audioManager == null) return;

        sfxMuted = !sfxMuted;
        sfxSlash.SetActive(sfxMuted);

        if (sfxMuted)
        {
            lastSFXVolume = sfxSlider.value;
            sfxSlider.value = 0;
            audioManager.sfxSource.volume = 0;
        }
        else
        {
            sfxSlider.value = lastSFXVolume;
            audioManager.sfxSource.volume = lastSFXVolume;
        }

        PlayerPrefs.SetInt("SFX_Muted", sfxMuted ? 1 : 0);
        PlayerPrefs.SetFloat("SFX_Volume", audioManager.sfxSource.volume);
    }

    void ToggleBGM()
    {
        if (audioManager == null) return;

        bgmMuted = !bgmMuted;
        bgmSlash.SetActive(bgmMuted);

        if (bgmMuted)
        {
            lastBGMVolume = bgmSlider.value;
            bgmSlider.value = 0;
            audioManager.musicSource.volume = 0;
        }
        else
        {
            bgmSlider.value = lastBGMVolume;
            audioManager.musicSource.volume = lastBGMVolume;
        }

        PlayerPrefs.SetInt("BGM_Muted", bgmMuted ? 1 : 0);
        PlayerPrefs.SetFloat("BGM_Volume", audioManager.musicSource.volume);
    }
}
