using UnityEngine;

public class EnemyHorizontal : BaseEnemyMovement
{
    private bool movingRight = true;
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
        float left = initialPosition.x - distance;
        float right = initialPosition.x + distance;

        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            // Âm thanh được điều khiển bởi CheckPlayerDistanceAndControlSound() trong BaseEnemyMovement
            if (transform.position.x >= right)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            // Âm thanh được điều khiển bởi CheckPlayerDistanceAndControlSound() trong BaseEnemyMovement
            if (transform.position.x <= left)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    private void PlaySnailSound()
    {
        // Không cần phát ở đây nữa - âm thanh sẽ được điều khiển bởi CheckPlayerDistanceAndControlSound() trong BaseEnemyMovement
        // Chỉ cần đảm bảo enemy đang di chuyển
    }

    // Không cần PlaySnailSoundNow() nữa - đã được xử lý trong BaseEnemyMovement

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Gọi từ Animation Event
    public void PlayMoveSFX()
    {
        if (moveSFX == null) return;
        if (soundController != null)
        {
            soundController.PlayOneShot3D(moveSFX);
        }
        else if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ConfigureEnemy3DSource(oneShotSource);
            oneShotSource.spatialBlend = 1f;
            oneShotSource.minDistance = 1f;
            oneShotSource.maxDistance = 3f;
            SoundManager.Instance.PlayEnemyOneShot3D(oneShotSource, moveSFX, 1f);
        }
    }
}
