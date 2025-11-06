using UnityEngine;

/// <summary>
/// Concrete class để quản lý âm thanh cho các enemy không kế thừa từ BaseEnemyMovement
/// (Như Frog/EnemyVertical)
/// </summary>
public class EnemySoundController : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioClip enemySoundClip; // Gán trực tiếp AudioClip vào đây
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.5f;
    
    [Header("3D Sound Settings")]
    [Tooltip("Khoảng cách tối thiểu để nghe âm thanh ở mức đầy đủ")]
    [SerializeField] private float minDistance = 1f;
    [Tooltip("Khoảng cách tối đa để nghe được âm thanh (beyond này sẽ không nghe thấy)")]
    [SerializeField] private float maxDistance = 8f;
    [Tooltip("Độ 3D của âm thanh (0 = 2D, 1 = 3D thuần)")]
    [Range(0f, 1f)]
    [SerializeField] private float spatialBlend = 0.75f;
    
    private AudioSource audioSource;
    private Transform playerTransform;
    private bool isPlayingSound = false;
    
    private void Start()
    {
        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            player = GameObject.Find("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        // Tạo hoặc lấy AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Cấu hình AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = spatialBlend;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.mute = false;
        audioSource.outputAudioMixerGroup = null;
        
        if (enemySoundClip != null)
        {
            audioSource.clip = enemySoundClip;
        }
    }
    
    private void Update()
    {
        if (playerTransform == null || audioSource == null || enemySoundClip == null)
            return;
        
        // Tính khoảng cách đến player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        // Nếu player ở trong khoảng cách nghe được
        if (distanceToPlayer <= maxDistance)
        {
            // Nếu chưa đang phát, bắt đầu phát
            if (!isPlayingSound)
            {
                StartPlayingSound();
            }
        }
        else
        {
            // Nếu player ở xa, dừng âm thanh
            if (isPlayingSound)
            {
                StopPlayingSound();
            }
        }
    }
    
    private void StartPlayingSound()
    {
        if (audioSource == null || enemySoundClip == null)
            return;
        
        // Tính volume với SFX volume từ SoundManager nếu có
        float finalVolume = soundVolume;
        if (SoundManager.Instance != null)
        {
            finalVolume = soundVolume * SoundManager.Instance.sfxVolume;
        }
        
        audioSource.clip = enemySoundClip;
        audioSource.volume = finalVolume;
        audioSource.loop = true;
        audioSource.spatialBlend = spatialBlend;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.mute = false;
        audioSource.outputAudioMixerGroup = null;
        
        audioSource.Play();
        isPlayingSound = true;
        
        Debug.Log($"[{gameObject.name}] 🔊 Started playing sound: {enemySoundClip.name} | Volume: {finalVolume}");
    }
    
    private void StopPlayingSound()
    {
        if (audioSource == null)
            return;
        
        audioSource.Stop();
        isPlayingSound = false;
        
        Debug.Log($"[{gameObject.name}] 🔇 Stopped playing sound (player is far)");
    }
}

