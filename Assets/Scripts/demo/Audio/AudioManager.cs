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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        yield return null; // Chờ 1 frame cho chắc chắn scene đã load xong
        string sceneName = newScene.name;

        // 🎮 Nếu là gameplay (Game, Level1, Level2...)
        if (sceneName == "Game" || (sceneName.StartsWith("Level") && sceneName != "LevelSelect"))
        {
            StopMusic();
            PlaySFX(game_start);
            yield return new WaitForSeconds(game_start.length);
            PlayMusic(gameplay_music);
        }
        // 🎵 Nếu là Menu, Shop hoặc LevelSelect
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

        // 🔧 Nếu clip khác hoặc chưa phát thì mới phát lại
        if (musicSource.clip != clip || !musicSource.isPlaying)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic() => musicSource.Stop();

    // ====== SFX ======
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

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
