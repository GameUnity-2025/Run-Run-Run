using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Player Sounds")]
    public AudioClip playerFootstepSound;
    public AudioClip playerJumpSound;
    public AudioClip gemCollectSound;
    public AudioClip waterSplashSound; // Âm thanh khi rơi xuống nước

    // Enemy sounds were migrated to per-enemy clips. No enemy clips are stored here anymore.

    [Header("Ambient Sounds")]
    public AudioClip waterAmbientSound; // Âm thanh nước chảy trong map

    [Header("Audio Source Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource; // AudioSource riêng cho ambient sounds

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    
    [Header("SFX Volume Multipliers")]
    [Range(0f, 1f)]
    [Tooltip("Volume chuẩn cho tất cả các âm thanh SFX (Player, Enemy, etc.)")]
    public float defaultSFXVolume = 0.5f; // Volume chuẩn cho tất cả SFX

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Tạo AudioSource nếu chưa có
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
            }

            // Tạo AudioSource cho ambient sounds nếu chưa có
            if (ambientSource == null)
            {
                ambientSource = gameObject.AddComponent<AudioSource>();
                ambientSource.playOnAwake = false;
                ambientSource.loop = true;
                ambientSource.spatialBlend = 0f; // 2D sound
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Cập nhật volume
        UpdateVolumes();
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void UpdateVolumes()
    {
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void PlayAmbientSound(AudioClip clip, float volume = 0.5f)
    {
        if (clip != null && ambientSource != null)
        {
            ambientSource.clip = clip;
            ambientSource.volume = volume;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }

    public void StopAmbientSound()
    {
        if (ambientSource != null)
        {
            ambientSource.Stop();
        }
    }

    public void SetAmbientVolume(float volume)
    {
        if (ambientSource != null)
        {
            ambientSource.volume = Mathf.Clamp01(volume);
        }
    }
}

