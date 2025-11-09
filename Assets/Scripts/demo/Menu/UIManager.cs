using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; // 🟢 cần để load scene Home

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;   // OptionsPanel prefab instance
    [SerializeField] private GameObject pausePanel;      // Panel tạm dừng
    [SerializeField] private GameObject backgroundDim;   // lớp nền mờ toàn màn hình

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
            backgroundDim.SetActive(false); // ẩn lớp mờ khi khởi động
    }

    // ======================================================
    // 🟢 SETTINGS
    // ======================================================
    public void ToggleSettings()
    {
        if (isSettingsOpen) CloseSettings();
        else OpenSettings();
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) return;

        // bật panel + lớp nền mờ
        settingsPanel.SetActive(true);
        if (backgroundDim != null)
            backgroundDim.SetActive(true);

        isSettingsOpen = true;
        PauseEverything();

        // cập nhật volume cho slider
        if (optionsController != null && AudioManager.Instance != null)
        {
            var am = AudioManager.Instance;
            optionsController.UpdateSliders(am.sfxSource.volume, am.musicSource.volume);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel == null) return;

        // tắt panel + lớp nền mờ
        settingsPanel.SetActive(false);
        if (backgroundDim != null)
            backgroundDim.SetActive(false);

        isSettingsOpen = false;
        ResumeEverything();
    }

    // ======================================================
    // 🟣 PAUSE MENU
    // ======================================================
    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (pausePanel == null) return;

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

        pausePanel.SetActive(false);
        if (backgroundDim != null)
            backgroundDim.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void GoHome()
    {
        // resume time trước khi load scene mới
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("Menu"); // 🏠 tên scene chính
    }

    // ======================================================
    // 🎚 AUDIO
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
    // ⏯ HỖ TRỢ DỪNG / TIẾP TỤC
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
