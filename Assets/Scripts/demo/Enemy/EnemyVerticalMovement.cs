using UnityEngine;

public class EnemyVerticalMovement : BaseEnemyMovement
{
    private bool movingUp = true;
    private bool hasPlayedFirstSound = false; // Để phát âm thanh ngay lần đầu
    [Header("Sound - Move SFX")]
    [SerializeField] private AudioClip moveSFX; // Clip phát theo Animator Event
    private EnemySoundController soundController;
    private AudioSource oneShotSource;

    protected override void Start()
    {
        base.Start();
        footstepTimer = 0f; // Đảm bảo timer bắt đầu từ 0
        hasPlayedFirstSound = false;
        soundController = GetComponent<EnemySoundController>();
        oneShotSource = GetComponent<AudioSource>();
        if (oneShotSource == null)
        {
            oneShotSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected override void Move()
    {
        float top = initialPosition.y + distance;
        float bottom = initialPosition.y - distance;

        if (movingUp)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            // Âm thanh được điều khiển bởi CheckPlayerDistanceAndControlSound() trong BaseEnemyMovement
            if (transform.position.y >= top) movingUp = false;
        }
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            // Âm thanh được điều khiển bởi CheckPlayerDistanceAndControlSound() trong BaseEnemyMovement
            if (transform.position.y <= bottom) movingUp = true;
        }
    }

    private void PlayBeeSound()
    {
        // Không cần phát ở đây nữa - âm thanh sẽ được điều khiển bởi CheckPlayerDistanceAndControlSound() trong BaseEnemyMovement
        // Chỉ cần đảm bảo enemy đang di chuyển
    }

    // Không cần PlayBeeSoundNow() nữa - đã được xử lý trong BaseEnemyMovement

    // Gọi từ Animation Event
    public void PlayMoveSFX()
    {
        if (moveSFX == null) return;
        if (soundController != null)
        {
            // cấu hình nguồn mặc định đã set trong controller; play one-shot 3D
            soundController.PlayOneShot3D(moveSFX);
        }
        else if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ConfigureEnemy3DSource(oneShotSource);
            // ép 3D thuần và tầm 1..3
            oneShotSource.spatialBlend = 1f;
            oneShotSource.minDistance = 1f;
            oneShotSource.maxDistance = 3f;
            SoundManager.Instance.PlayEnemyOneShot3D(oneShotSource, moveSFX, 1f);
        }
    }
}
