using UnityEngine;

/// <summary>
/// Script phát âm thanh nước chảy tại vị trí cụ thể
/// Đặt GameObject này vào vị trí nước trong scene để phát âm thanh
/// </summary>
public class WaterSoundEmitter : MonoBehaviour
{
    [Header("Sound Settings")]
    [Tooltip("Volume của âm thanh (0-1). Sẽ được nhân với SFX Volume từ SoundManager")]
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.4f;
    
    [Header("3D Sound Settings")]
    [Tooltip("Khoảng cách tối thiểu để nghe âm thanh ở mức đầy đủ")]
    [SerializeField] private float minDistance = 2f;
    [Tooltip("Khoảng cách tối đa để nghe được âm thanh")]
    [SerializeField] private float maxDistance = 10f;
    [Tooltip("Độ 3D của âm thanh (0 = 2D, 1 = 3D thuần). 0.75 là giá trị tốt")]
    [Range(0f, 1f)]
    [SerializeField] private float spatialBlend = 0.75f;
    
    private AudioSource audioSource;

    private void Start()
    {
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
        audioSource.dopplerLevel = 0f;
        audioSource.mute = false;
        audioSource.outputAudioMixerGroup = null;
        
        // Lấy clip từ SoundManager
        if (SoundManager.Instance != null && SoundManager.Instance.waterAmbientSound != null)
        {
            audioSource.clip = SoundManager.Instance.waterAmbientSound;
            
            // Tính volume với SFX volume từ SoundManager
            float finalVolume = soundVolume;
            if (SoundManager.Instance != null)
            {
                finalVolume = soundVolume * SoundManager.Instance.sfxVolume;
            }
            audioSource.volume = finalVolume;
            
            // Phát âm thanh
            audioSource.Play();
            Debug.Log($"[{gameObject.name}] 🔊 Started playing water sound at position: {transform.position}");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] ⚠️ Water Ambient Sound chưa được gán trong SoundManager!");
        }
    }
    
    private void Update()
    {
        // Cập nhật volume nếu SFX volume thay đổi
        if (audioSource != null && audioSource.isPlaying && SoundManager.Instance != null)
        {
            float finalVolume = soundVolume * SoundManager.Instance.sfxVolume;
            audioSource.volume = finalVolume;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Vẽ khoảng cách min/max để dễ visualize trong Scene view
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}

