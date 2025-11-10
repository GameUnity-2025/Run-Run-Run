using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider sfxSlider;
    public Slider bgmSlider;
    public Button sfxButton;
    public Button bgmButton;

    [Header("Button Images")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private Image sfxImage;
    private Image bgmImage;

    private bool sfxMuted = false;
    private bool bgmMuted = false;
    private float lastSFXVolume = 1f;
    private float lastBGMVolume = 1f;

    private AudioManager audioManager;

    void OnEnable()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null) return;

        // ✅ Load trạng thái từ AudioManager
        sfxMuted = audioManager.IsSFXMuted();
        bgmMuted = audioManager.IsMusicMuted();
        lastSFXVolume = audioManager.GetSFXVolume();
        lastBGMVolume = audioManager.GetMusicVolume();

        // Setup buttons
        if (sfxButton != null)
        {
            sfxImage = sfxButton.GetComponent<Image>();
            sfxButton.onClick.RemoveAllListeners();
            sfxButton.onClick.AddListener(ToggleSFX);
        }

        if (bgmButton != null)
        {
            bgmImage = bgmButton.GetComponent<Image>();
            bgmButton.onClick.RemoveAllListeners();
            bgmButton.onClick.AddListener(ToggleBGM);
        }

        // Setup sliders
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.value = sfxMuted ? 0 : lastSFXVolume; // ✅ Hiển thị đúng giá trị
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.value = bgmMuted ? 0 : lastBGMVolume; // ✅ Hiển thị đúng giá trị
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        UpdateButtonIcons();
    }

    void SetSFXVolume(float value)
    {
        if (audioManager == null) return;

        if (!sfxMuted)
        {
            lastSFXVolume = value;
            audioManager.SetSFXVolume(value); // ✅ Gọi hàm AudioManager để lưu
        }
    }

    void SetBGMVolume(float value)
    {
        if (audioManager == null) return;

        if (!bgmMuted)
        {
            lastBGMVolume = value;
            audioManager.SetMusicVolume(value); // ✅ Gọi hàm AudioManager để lưu
        }
    }

    public void UpdateSliders(float sfxValue, float bgmValue)
    {
        if (sfxSlider != null)
            sfxSlider.value = sfxValue;

        if (bgmSlider != null)
            bgmSlider.value = bgmValue;

        UpdateButtonIcons();
    }

    void ToggleSFX()
    {
        if (audioManager == null) return;

        sfxMuted = !sfxMuted;

        if (sfxMuted)
        {
            lastSFXVolume = sfxSlider.value;
            sfxSlider.value = 0;
        }
        else
        {
            sfxSlider.value = lastSFXVolume;
        }

        audioManager.SetSFXMute(sfxMuted); // ✅ Gọi hàm AudioManager để lưu
        UpdateButtonIcons();
    }

    void ToggleBGM()
    {
        if (audioManager == null) return;

        bgmMuted = !bgmMuted;

        if (bgmMuted)
        {
            lastBGMVolume = bgmSlider.value;
            bgmSlider.value = 0;
        }
        else
        {
            bgmSlider.value = lastBGMVolume;
        }

        audioManager.SetMusicMute(bgmMuted); // ✅ Gọi hàm AudioManager để lưu
        UpdateButtonIcons();
    }

    void UpdateButtonIcons()
    {
        if (sfxImage != null)
            sfxImage.sprite = sfxMuted ? soundOffSprite : soundOnSprite;

        if (bgmImage != null)
            bgmImage.sprite = bgmMuted ? soundOffSprite : soundOnSprite;
    }

    public void OnBackButtonPressed()
    {
        // ✅ Phát SFX khi nhấn back
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play_MenuBack();
        }

        UIManager ui = FindAnyObjectByType<UIManager>();
        if (ui != null)
            ui.CloseSettings();
        else
            gameObject.SetActive(false);
    }
}