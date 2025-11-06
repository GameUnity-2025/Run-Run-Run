using UnityEngine;

public class WaterSoundZone : MonoBehaviour
{
    [Header("Water Sound Settings")]
    [SerializeField] private float soundVolume = 0.4f; // Volume khi player ở trong vùng
    [SerializeField] private float fadeInSpeed = 2f; // Tốc độ tăng volume
    [SerializeField] private float fadeOutSpeed = 2f; // Tốc độ giảm volume
    [SerializeField] private bool useDistanceBasedVolume = true; // Sử dụng volume dựa trên khoảng cách
    [SerializeField] private float maxDistance = 10f; // Khoảng cách tối đa để nghe rõ
    [SerializeField] private float minDistance = 2f; // Khoảng cách tối đa để volume đạt max

    private AudioSource waterAudioSource;
    private bool playerInZone = false;
    private Transform playerTransform;
    private float currentVolume = 0f;
    private float targetVolume = 0f;

    private void Start()
    {
        // Tạo AudioSource riêng cho zone này
        waterAudioSource = gameObject.AddComponent<AudioSource>();
        waterAudioSource.playOnAwake = false;
        waterAudioSource.loop = true;
        waterAudioSource.spatialBlend = 1f; // 3D sound
        waterAudioSource.rolloffMode = AudioRolloffMode.Linear;
        waterAudioSource.minDistance = minDistance;
        waterAudioSource.maxDistance = maxDistance;
        waterAudioSource.volume = 0f;

        // Gán clip từ SoundManager
        if (SoundManager.Instance != null && SoundManager.Instance.waterAmbientSound != null)
        {
            waterAudioSource.clip = SoundManager.Instance.waterAmbientSound;
        }
        else
        {
            Debug.LogWarning("Water Ambient Sound chưa được gán trong SoundManager!");
        }

        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (waterAudioSource == null || waterAudioSource.clip == null) return;

        // Tính toán volume dựa trên khoảng cách nếu enabled
        if (useDistanceBasedVolume && playerTransform != null && playerInZone)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
            targetVolume = soundVolume * (1f - normalizedDistance);
        }
        else if (playerInZone)
        {
            targetVolume = soundVolume;
        }
        else
        {
            targetVolume = 0f;
        }

        // Fade in/out volume
        if (currentVolume < targetVolume)
        {
            currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, fadeInSpeed * Time.deltaTime);
        }
        else if (currentVolume > targetVolume)
        {
            currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, fadeOutSpeed * Time.deltaTime);
        }

        waterAudioSource.volume = currentVolume;

        // Bật/tắt AudioSource dựa trên volume
        if (currentVolume > 0.01f && !waterAudioSource.isPlaying)
        {
            waterAudioSource.Play();
        }
        else if (currentVolume <= 0.01f && waterAudioSource.isPlaying)
        {
            waterAudioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ gizmo để hiển thị vùng trigger
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            if (col is BoxCollider2D)
            {
                BoxCollider2D boxCol = col as BoxCollider2D;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(boxCol.offset, boxCol.size);
            }
            else if (col is CircleCollider2D)
            {
                CircleCollider2D circleCol = col as CircleCollider2D;
                Gizmos.DrawSphere(transform.position + (Vector3)circleCol.offset, circleCol.radius * transform.lossyScale.x);
            }
        }

        // Vẽ khoảng cách min/max nếu sử dụng distance-based volume
        if (useDistanceBasedVolume)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, minDistance);
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }
    }
}

