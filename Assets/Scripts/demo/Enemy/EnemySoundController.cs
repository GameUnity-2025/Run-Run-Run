using UnityEngine;

/// <summary>
/// Concrete class để quản lý âm thanh cho các enemy không kế thừa từ BaseEnemyMovement
/// (Như Frog/EnemyVertical)
/// </summary>
public class EnemySoundController : MonoBehaviour
{
    [Header("Sound Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.5f;
    
    [Header("3D Sound Settings")]
    [Tooltip("Khoảng cách tối thiểu để nghe âm thanh ở mức đầy đủ")]
    [SerializeField] private float minDistance = 1f;
    [Tooltip("Khoảng cách tối đa để nghe được âm thanh (beyond này sẽ không nghe thấy)")]
    [SerializeField] private float maxDistance = 3f;
    [Tooltip("Độ 3D của âm thanh (0 = 2D, 1 = 3D thuần)")]
    [Range(0f, 1f)]
    [SerializeField] private float spatialBlend = 0.75f;
    [Header("Behavior")]
    [Tooltip("Bật để phát liên tục khi player ở gần. Tắt để chỉ phát theo lệnh (ví dụ Frog Jump)")]
    [SerializeField] private bool continuousLoop = true;
    
    private AudioSource audioSource;
    [Header("Player Reference")]
    [Tooltip("Tag của Player để dò tìm. Mặc định là 'Player'.")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Có thể gán trực tiếp Transform Player tại đây để bỏ qua tìm kiếm bằng tag.")]
    [SerializeField] private Transform playerTransform;
    private bool isPlayingSound = false;
    
    private void Start()
    {
        // Tìm player
        GameObject player = null;
        if (playerTransform == null && !string.IsNullOrEmpty(playerTag))
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
        }
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
        
        // Không gán clip ở đây nữa - dùng Animator Events để truyền clip vào
    }
    
    private void Update()
    {
        if (!continuousLoop) return;

        if (playerTransform == null || audioSource == null)
            return;
        
        // Tính khoảng cách đến player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        // Nếu player ở trong khoảng cách nghe được
        if (distanceToPlayer <= maxDistance)
        {
            // Nếu chưa đang phát, bắt đầu phát
            if (!isPlayingSound)
            {
                // Nếu không có clip sẵn, chờ Animator Event gọi StartLoop
                if (audioSource.clip != null)
                {
                    StartPlayingSound();
                }
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
        if (audioSource == null || audioSource.clip == null)
            return;
        
        // Tính volume với SFX volume từ SoundManager nếu có
        float finalVolume = soundVolume;
        if (SoundManager.Instance != null)
        {
            finalVolume = soundVolume * SoundManager.Instance.sfxVolume;
        }
        
        audioSource.volume = finalVolume;
        audioSource.loop = true;
        audioSource.spatialBlend = spatialBlend;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.mute = false;
        audioSource.outputAudioMixerGroup = null;
        
        audioSource.Play();
        isPlayingSound = true;
        
        Debug.Log($"[{gameObject.name}] 🔊 Started playing sound: {audioSource.clip?.name} | Volume: {finalVolume}");
    }
    
    private void StopPlayingSound()
    {
        if (audioSource == null)
            return;
        
        audioSource.Stop();
        isPlayingSound = false;
        
        Debug.Log($"[{gameObject.name}] 🔇 Stopped playing sound (player is far)");
    }

    public void PlayOneShot3D(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        // cấu hình 3D theo mặc định hiện tại
        audioSource.playOnAwake = false;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.spatialBlend = spatialBlend;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.loop = false;

        float finalVolume = soundVolume;
        if (SoundManager.Instance != null)
        {
            finalVolume = soundVolume * SoundManager.Instance.sfxVolume;
        }
        audioSource.PlayOneShot(clip, finalVolume);
    }

    // === Animator-driven API ===
    public void StartLoop(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.clip = clip;
        StartPlayingSound();
    }

    public void StopLoop()
    {
        StopPlayingSound();
    }
}


