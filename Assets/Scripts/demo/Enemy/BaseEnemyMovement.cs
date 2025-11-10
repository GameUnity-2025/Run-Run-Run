using UnityEngine;

public enum EnemySoundType
{
    Snail, // Enemy Ốc sên (di chuyển ngang)
    Bee,   // Enemy Ong (di chuyển dọc)
    Frog   // Enemy Ếch (nhảy)
}

public abstract class BaseEnemyMovement : MonoBehaviour
{
    public static bool GlobalMuteEnemies = true;

    [Header("Common Movement Settings")]
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float distance = 3f;

    [Header("Sound Settings")]
    public AudioClip enemySoundClip; // Gán trực tiếp AudioClip vào đây (không cần SoundManager)
    [SerializeField] protected float footstepInterval = 0.5f; // Khoảng thời gian giữa các bước chân
    [SerializeField] protected bool useIndividualAudioSource = true; // Mỗi enemy có AudioSource riêng
    [Range(0f, 1f)]
    [SerializeField] protected float soundVolume = 0.5f; // Volume cho âm thanh này (có thể điều chỉnh riêng cho từng enemy)
    [Tooltip("Bật để BaseEnemyMovement tự phát loop theo khoảng cách. Tắt để dùng Animator/EnemySoundController (như Frog).")]
    [SerializeField] protected bool enableAutoLoopAudio = true;
    [Tooltip("Luôn bám theo Player gần nhất có tag chỉ định để tránh trỏ nhầm SpawnPoint/placeholder.")]
    [SerializeField] private bool alwaysTrackClosestPlayer = true;
    [Header("3D Sound Settings")]
    [Tooltip("Khoảng cách tối thiểu để nghe âm thanh ở mức đầy đủ")]
    [SerializeField] protected float minDistance = 1f; // Khoảng cách tối thiểu
    [Tooltip("Khoảng cách tối đa để nghe được âm thanh (beyond này sẽ không nghe thấy)")]
    [SerializeField] protected float maxDistance = 3f; // Khoảng cách tối đa
    [Tooltip("Biên độ trễ dừng âm (hysteresis) để tránh ngắt-quãng khi đứng ở rìa phạm vi nghe")]
    [SerializeField] protected float distanceHysteresis = 0.5f;
    [Tooltip("Độ 3D của âm thanh (0 = 2D, 1 = 3D thuần). Giá trị 0.7-0.8 là tốt để có khoảng cách nhưng vẫn nghe được")]
    [Range(0f, 1f)]
    [SerializeField] protected float spatialBlend = 0.75f; // Mix 2D/3D (0.75 = chủ yếu 3D nhưng vẫn nghe được từ xa)
    
    // Public getters để các class khác có thể truy cập
    public float GetSoundVolume() => soundVolume;
    public float GetMinDistance() => minDistance;
    public float GetMaxDistance() => maxDistance;
    public float GetSpatialBlend() => spatialBlend;
    
    // Public method để các class khác có thể gọi kiểm tra khoảng cách
    public void CheckPlayerDistance()
    {
        CheckPlayerDistanceAndControlSound();
    }
    
    protected float footstepTimer = 0f;
    protected AudioSource enemyAudioSource; // AudioSource riêng cho mỗi enemy
    protected bool wasMoving = false; // Theo dõi trạng thái di chuyển trước đó
    protected bool isPlayingSound = false; // Theo dõi xem có đang phát âm thanh không

    protected Vector3 initialPosition;
    [Header("Player Reference")]
    [Tooltip("Tag của Player để dò tìm. Mặc định là 'Player'.")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Có thể gán trực tiếp Transform Player tại đây để bỏ qua tìm kiếm bằng tag.")]
    [SerializeField] protected Transform playerTransform; // Transform của player để kiểm tra khoảng cách

    protected virtual void Start()
    {
        initialPosition = transform.position;
        
        // Tìm player trong scene
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
            // Fallback: tìm bằng tên
            player = GameObject.Find("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        Debug.Log($"[{gameObject.name}] BaseEnemyMovement.Start() called | useIndividualAudioSource: {useIndividualAudioSource} | Player found: {(playerTransform != null ? "Yes" : "No")}");
        
        // Nếu dùng auto loop, kiểm tra clip; nếu không thì bỏ qua cảnh báo
        if (enableAutoLoopAudio)
        {
            if (enemySoundClip == null)
            {
                Debug.LogError($"[{gameObject.name}] ❌ CRITICAL: EnemySoundClip is NULL! Please assign AudioClip in Inspector on the BaseEnemyMovement component.");
            }
            else
            {
                Debug.Log($"[{gameObject.name}] ✓ EnemySoundClip assigned: {enemySoundClip.name}");
            }
        }
        
        // Tạo AudioSource riêng cho enemy này để tránh trùng âm thanh
        if (useIndividualAudioSource)
        {
            // Kiểm tra xem đã có AudioSource chưa
            enemyAudioSource = GetComponent<AudioSource>();
            if (enemyAudioSource == null)
            {
                Debug.Log($"[{gameObject.name}] Creating new AudioSource component...");
                enemyAudioSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Using existing AudioSource component");
            }
            
            // Cấu hình AudioSource - ĐẢM BẢO CẤU HÌNH ĐÚNG
            ConfigureAudioSource();
            
            // Gán clip nếu dùng auto loop
            if (enableAutoLoopAudio && enemySoundClip != null)
            {
                enemyAudioSource.clip = enemySoundClip;
            }
            
            Debug.Log($"[{gameObject.name}] AudioSource configured | Mute: {enemyAudioSource.mute} | Volume: {enemyAudioSource.volume} | SpatialBlend: {enemyAudioSource.spatialBlend} | Loop: {enemyAudioSource.loop} | Clip: {(enemyAudioSource.clip != null ? enemyAudioSource.clip.name : "NULL")}");
            
            // Khởi tạo timer và wasMoving
            footstepTimer = 0f;
            wasMoving = false;
        }
        else
        {
            Debug.Log($"[{gameObject.name}] useIndividualAudioSource is false, will use SoundManager");
        }
    }
    
    /// <summary>
    /// Cấu hình AudioSource với các thiết lập tối ưu để đảm bảo âm thanh phát được
    /// </summary>
    protected virtual void ConfigureAudioSource()
    {
        if (enemyAudioSource == null) return;
        
        enemyAudioSource.playOnAwake = false;
        enemyAudioSource.loop = true; // LOOP để âm thanh phát liên tục không bị ngắt
        enemyAudioSource.spatialBlend = spatialBlend; // Sử dụng giá trị từ Inspector
        enemyAudioSource.rolloffMode = AudioRolloffMode.Linear;
        enemyAudioSource.minDistance = minDistance; // 1f
        enemyAudioSource.maxDistance = maxDistance; // 3f
        enemyAudioSource.dopplerLevel = 0f; // tránh méo tiếng khi di chuyển
        enemyAudioSource.mute = false; // Đảm bảo không bị mute
        enemyAudioSource.volume = 1f; // Sẽ nhân với soundVolume khi phát
        enemyAudioSource.outputAudioMixerGroup = null; // Đảm bảo không bị mute bởi mixer
        enemyAudioSource.bypassEffects = false;
        enemyAudioSource.bypassListenerEffects = false;
        enemyAudioSource.bypassReverbZones = false;
    }

    protected virtual void Update()
    {
        Move();

        // Nếu đang mute toàn cục => dừng phát âm thanh luôn
        if (GlobalMuteEnemies)
        {
            if (isPlayingSound && enemyAudioSource != null)
            {
                enemyAudioSource.Stop();
                isPlayingSound = false;
            }
            return; // Bỏ qua xử lý âm thanh
        }

        if (enableAutoLoopAudio)
        {
            CheckPlayerDistanceAndControlSound();
        }
    }

    /*protected virtual void Update()
    {
        Move(); // gọi hàm abstract mà class con sẽ cài đặt
        
        // Nếu auto loop bật, kiểm tra khoảng cách và điều khiển âm thanh
        if (enableAutoLoopAudio)
        {
            CheckPlayerDistanceAndControlSound();
        }
    }*/
    
    /// <summary>
    /// Kiểm tra khoảng cách với player và phát/dừng âm thanh dựa trên khoảng cách
    /// </summary>
    protected virtual void CheckPlayerDistanceAndControlSound()
    {
        // Nếu không có AudioSource hoặc AudioClip, không làm gì
        if (enemyAudioSource == null || enemySoundClip == null)
            return;
        
        // Nếu luôn theo dõi player gần nhất hoặc chưa có player, tìm lại
        if (alwaysTrackClosestPlayer || playerTransform == null)
        {
            playerTransform = FindClosestPlayer();
            
            // Nếu vẫn không tìm thấy, không làm gì
            if (playerTransform == null)
            {
                Debug.LogWarning($"[{gameObject.name}] ⚠️ Player not found! Cannot check distance.");
                return;
            }
        }
        
        // Tính khoảng cách đến player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        // Nếu player ở trong khoảng cách nghe được -> bắt đầu phát
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
            // Chỉ dừng khi vượt quá (maxDistance + hysteresis) để tránh stop/start liên tục ở rìa
            if (isPlayingSound && distanceToPlayer > (maxDistance + Mathf.Max(0f, distanceHysteresis)))
            {
                StopPlayingSound();
            }
        }
    }
    
    /// <summary>
    /// Bắt đầu phát âm thanh
    /// </summary>
    protected virtual void StartPlayingSound()
    {
        if (enemyAudioSource == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Cannot start sound - enemyAudioSource is NULL");
            return;
        }
        
        if (enemySoundClip == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Cannot start sound - enemySoundClip is NULL");
            return;
        }
        
        // Tính volume với SFX volume từ SoundManager nếu có
        float finalVolume = soundVolume;
        if (SoundManager.Instance != null)
        {
            finalVolume = soundVolume * SoundManager.Instance.sfxVolume;
        }
        
        // Đảm bảo AudioSource được cấu hình đúng
        ConfigureAudioSource();
        
        enemyAudioSource.clip = enemySoundClip;
        enemyAudioSource.volume = finalVolume;
        enemyAudioSource.loop = true;
        enemyAudioSource.spatialBlend = spatialBlend;
        enemyAudioSource.minDistance = minDistance;
        enemyAudioSource.maxDistance = maxDistance;
        enemyAudioSource.mute = false;
        enemyAudioSource.outputAudioMixerGroup = null;
        
        enemyAudioSource.Play();
        isPlayingSound = true;
        
        Debug.Log($"[{gameObject.name}] 🔊 Started playing sound: {enemySoundClip.name} | Volume: {finalVolume} | Player distance: {Vector3.Distance(transform.position, playerTransform != null ? playerTransform.position : Vector3.zero)}");
    }
    
    /// <summary>
    /// Dừng phát âm thanh
    /// </summary>
    protected virtual void StopPlayingSound()
    {
        if (enemyAudioSource == null)
            return;
        
        enemyAudioSource.Stop();
        isPlayingSound = false;
        
        Debug.Log($"[{gameObject.name}] 🔇 Stopped playing sound (player is far)");
    }

    protected abstract void Move(); // hàm abstract (chưa định nghĩa)

    protected virtual void PlayEnemyFootstepSound()
    {
        // Phát âm thanh ngay khi bắt đầu di chuyển
        if (!wasMoving)
        {
            footstepTimer = 0f; // Reset timer
            PlayEnemySoundNow(); // Phát ngay lập tức
            wasMoving = true;
        }
        
        // Tiếp tục phát theo interval
        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepInterval)
        {
            PlayEnemySoundNow();
            footstepTimer = 0f;
        }
    }

    protected virtual void StopEnemyFootstepSound()
    {
        wasMoving = false;
        footstepTimer = 0f;
        // Có thể dừng âm thanh nếu cần (nếu đang dùng loop)
    }

    protected virtual void PlayEnemySoundNow()
    {
        // Sử dụng AudioClip trực tiếp từ Inspector
        if (enemySoundClip == null)
        {
            Debug.LogWarning($"[{gameObject.name}] EnemySoundClip is NULL! Please assign AudioClip in Inspector.");
            return;
        }
        
        // Tính volume với SFX volume từ SoundManager nếu có
        float finalVolume = soundVolume;
        if (SoundManager.Instance != null)
        {
            finalVolume = soundVolume * SoundManager.Instance.sfxVolume;
        }
        
        // Sử dụng AudioSource riêng nếu được bật
        if (useIndividualAudioSource)
        {
            // Kiểm tra lại AudioSource
            if (enemyAudioSource == null)
            {
                enemyAudioSource = GetComponent<AudioSource>();
                if (enemyAudioSource == null)
                {
                    enemyAudioSource = gameObject.AddComponent<AudioSource>();
                    enemyAudioSource.playOnAwake = false;
                    enemyAudioSource.loop = true; // LOOP để âm thanh phát liên tục
                    enemyAudioSource.spatialBlend = spatialBlend; // Sử dụng giá trị từ Inspector
                    enemyAudioSource.rolloffMode = AudioRolloffMode.Linear;
                    enemyAudioSource.minDistance = minDistance;
                    enemyAudioSource.maxDistance = maxDistance;
                    enemyAudioSource.volume = 1f;
                    enemyAudioSource.outputAudioMixerGroup = null;
                }
            }
            
            // Gán clip và phát
            enemyAudioSource.clip = enemySoundClip;
            enemyAudioSource.volume = finalVolume;
            
            // Đảm bảo AudioSource không bị muted
            if (enemyAudioSource.mute)
            {
                enemyAudioSource.mute = false;
            }
            
            enemyAudioSource.Play();
        }
        // Fallback: dùng SoundManager nếu không có AudioSource riêng
        else if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(enemySoundClip, soundVolume);
        }
    }

    private Transform FindClosestPlayer()
    {
        if (string.IsNullOrEmpty(playerTag)) return null;
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        if (players == null || players.Length == 0)
        {
            GameObject byName = GameObject.Find("Player");
            return byName != null ? byName.transform : null;
        }
        float bestDist = float.MaxValue;
        Transform best = null;
        Vector3 myPos = transform.position;
        foreach (var go in players)
        {
            if (go == null || !go.activeInHierarchy) continue;
            float d = (go.transform.position - myPos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = go.transform;
            }
        }
        return best;
    }

}