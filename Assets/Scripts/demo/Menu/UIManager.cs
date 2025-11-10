using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;   // Settings panel prefab
    [SerializeField] private GameObject pausePanel;      // Pause menu
    [SerializeField] private GameObject backgroundDim;   // Màn nền mờ

    [Header("Audio")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private string masterVolumeParam = "MasterVolume";

    private bool isSettingsOpen = false;
    private bool isPaused = false;
    private OptionsMenuController optionsController;

    private void Awake()
    {
        // đảm bảo tất cả panel ẩn khi khởi động
        if (settingsPanel)
        {
            settingsPanel.SetActive(false);
            optionsController = settingsPanel.GetComponent<OptionsMenuController>();
        }

        if (pausePanel)
            pausePanel.SetActive(false);

        if (backgroundDim)
            backgroundDim.SetActive(false);
    }

    // ======================================================
    // ⚙ SETTINGS
    // ======================================================
    public void ToggleSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play_Button();
        }

        if (isSettingsOpen) CloseSettings();
        else OpenSettings();
    }

    public void OpenSettings()
    {
        if (isPaused || settingsPanel == null) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play_Button();
        }

        settingsPanel.SetActive(true);
        if (backgroundDim != null)
            backgroundDim.SetActive(true);

        isSettingsOpen = true;
        PauseEverything();

        // ✅ đảm bảo OnEnable của OptionsMenuController chạy xong trước khi cập nhật volume
        if (optionsController == null)
            optionsController = settingsPanel.GetComponent<OptionsMenuController>();

        if (optionsController != null && AudioManager.Instance != null)
        {
            var am = AudioManager.Instance;
            optionsController.UpdateSliders(am.sfxSource.volume, am.musicSource.volume);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel == null) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play_Button();
        }

        settingsPanel.SetActive(false);
        if (backgroundDim != null)
            backgroundDim.SetActive(false);

        isSettingsOpen = false;
        ResumeEverything();
    }

    // ======================================================
    // ⏸ PAUSE MENU
    // ======================================================
    public void TogglePause()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play_Button();
        }

        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (isSettingsOpen || pausePanel == null) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play_Button();
        }

        pausePanel.SetActive(true);
        if (backgroundDim != null)
            backgroundDim.SetActive(true);

        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        if (pausePanel == null) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play_Button();
        }

        pausePanel.SetActive(false);
        if (backgroundDim != null)
            backgroundDim.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void GoHome()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play_Button();
        }

        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("Menu");
    }

    // ======================================================
    // 🔊 AUDIO
    // ======================================================
    public void SetMasterVolume(float volume)
    {
        if (masterMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            masterMixer.SetFloat(masterVolumeParam, dB);
        }
    }

    // ======================================================
    // ⏯ SUPPORT
    // ======================================================
    private void PauseEverything()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    private void ResumeEverything()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
