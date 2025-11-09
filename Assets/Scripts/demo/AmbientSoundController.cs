using UnityEngine;

/// <summary>
/// Script này đã được thay thế bằng WaterSoundZone.cs
/// Sử dụng WaterSoundZone trên các khu vực nước thay vì script này
/// </summary>
public class AmbientSoundController : MonoBehaviour
{
    [Header("⚠️ DEPRECATED - Sử dụng WaterSoundZone thay thế")]
    [Header("Ambient Sound Settings")]
    [SerializeField] private bool playWaterSound = false; // Tắt mặc định, sử dụng WaterSoundZone thay thế
    [SerializeField] [Range(0f, 1f)] private float waterSoundVolume = 0.3f;

    private void Start()
    {
        // Phát âm thanh nước chảy khi map bắt đầu (chỉ nếu playWaterSound = true)
        // Khuyến nghị: Sử dụng WaterSoundZone thay vì script này
        if (playWaterSound && SoundManager.Instance != null)
        {
            if (SoundManager.Instance.waterAmbientSound != null)
            {
                SoundManager.Instance.PlayAmbientSound(
                    SoundManager.Instance.waterAmbientSound, 
                    waterSoundVolume
                );
            }
            else
            {
                Debug.LogWarning("Water Ambient Sound chưa được gán trong SoundManager!");
            }
        }
    }

    private void OnDestroy()
    {
        // Dừng âm thanh ambient khi scene bị destroy (nếu cần)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopAmbientSound();
        }
    }
}

