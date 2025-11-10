using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip menu_music;      // Menu, Shop, LevelSelect
    public AudioClip gameplay_music;  // Level1, Level2, Game...

    [Header("Sound Effects")]
    public AudioClip button;
    public AudioClip cartoon_fail_trumpet;
    public AudioClip game_start;
    public AudioClip mission_success;
    public AudioClip purchase_success;
    public AudioClip menu_back;
    public AudioClip spring_bounce;
    public AudioClip coin;
    public AudioClip jump;

    // ✅ Biến private để lưu trạng thái
    private float sfxVolume = 1f;
    private float musicVolume = 1f;
    private bool sfxMuted = false;
    private bool musicMuted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // ✅ Load settings ngay khi khởi động
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        PlayMusicForCurrentScene();
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        StopAllCoroutines();
        StartCoroutine(HandleSceneChange(newScene));
    }

    private IEnumerator HandleSceneChange(Scene newScene)
    {
        yield return null;
        string sceneName = newScene.name;

        if (sceneName == "Game" || (sceneName.StartsWith("Level") && sceneName != "LevelSelect"))
        {
            StopMusic();
            PlaySFX(game_start);
            yield return new WaitForSeconds(game_start.length);
            PlayMusic(gameplay_music);
        }
        else if (sceneName == "Menu" || sceneName == "Shop" || sceneName == "LevelSelect")
        {
            PlaySFX(menu_back);
            yield return new WaitForSeconds(menu_back.length);
            PlayMusic(menu_music);
        }
    }

    private void PlayMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Menu" || sceneName == "Shop" || sceneName == "LevelSelect")
            PlayMusic(menu_music);
        else if (sceneName == "Game" || (sceneName.StartsWith("Level") && sceneName != "LevelSelect"))
            PlayMusic(gameplay_music);
    }

    // ====== MUSIC ======
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip != clip || !musicSource.isPlaying)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.volume = musicMuted ? 0 : musicVolume; // ✅ Áp dụng volume đã lưu
            musicSource.Play();
        }
    }

    public void StopMusic() => musicSource.Stop();

    // ====== SFX ======
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.volume = sfxMuted ? 0 : sfxVolume; // ✅ Áp dụng volume đã lưu
        sfxSource.PlayOneShot(clip);
    }

    // ====== VOLUME CONTROL (gọi từ OptionsMenuController) ======
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxMuted ? 0 : sfxVolume;
        PlayerPrefs.SetFloat("SFX_Volume", sfxVolume);
        PlayerPrefs.Save(); // ✅ Lưu ngay
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicMuted ? 0 : musicVolume;
        PlayerPrefs.SetFloat("BGM_Volume", musicVolume);
        PlayerPrefs.Save(); // ✅ Lưu ngay
    }

    public void SetSFXMute(bool mute)
    {
        sfxMuted = mute;
        sfxSource.volume = mute ? 0 : sfxVolume;
        PlayerPrefs.SetInt("SFX_Muted", mute ? 1 : 0);
        PlayerPrefs.Save(); // ✅ Lưu ngay
    }

    public void SetMusicMute(bool mute)
    {
        musicMuted = mute;
        musicSource.volume = mute ? 0 : musicVolume;
        PlayerPrefs.SetInt("BGM_Muted", mute ? 1 : 0);
        PlayerPrefs.Save(); // ✅ Lưu ngay
    }

    // ✅ Load settings từ PlayerPrefs
    private void LoadSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        musicVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        sfxMuted = PlayerPrefs.GetInt("SFX_Muted", 0) == 1;
        musicMuted = PlayerPrefs.GetInt("BGM_Muted", 0) == 1;

        sfxSource.volume = sfxMuted ? 0 : sfxVolume;
        musicSource.volume = musicMuted ? 0 : musicVolume;

        Debug.Log($"✅ Loaded Settings - SFX: {sfxVolume} (Muted: {sfxMuted}), BGM: {musicVolume} (Muted: {musicMuted})");
    }

    // ✅ Getter để OptionsMenuController lấy giá trị
    public float GetSFXVolume() => sfxVolume;
    public float GetMusicVolume() => musicVolume;
    public bool IsSFXMuted() => sfxMuted;
    public bool IsMusicMuted() => musicMuted;

    // ====== Convenience ======
    public void Play_Button() => PlaySFX(button);
    public void Play_Fail() => PlaySFX(cartoon_fail_trumpet);
    public void Play_GameStart() => PlaySFX(game_start);
    public void Play_Success() => PlaySFX(mission_success);
    public void Play_Purchase() => PlaySFX(purchase_success);
    public void Play_Bounce() => PlaySFX(spring_bounce);
    public void Play_Coin() => PlaySFX(coin);
    public void Play_Jump() => PlaySFX(jump);
    public void Play_MenuBack() => PlaySFX(menu_back);
}