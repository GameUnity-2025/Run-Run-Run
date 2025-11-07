using UnityEngine;

public class EnemyHorizontal : BaseEnemyMovement
{
    private bool movingRight = true;
    private bool hasPlayedFirstSound = false; // Để phát âm thanh ngay lần đầu

    protected override void Start()
    {
        base.Start();
        footstepTimer = 0f; // Đảm bảo timer bắt đầu từ 0
        hasPlayedFirstSound = false;
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
}
