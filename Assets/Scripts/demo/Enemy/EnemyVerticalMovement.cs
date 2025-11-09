using UnityEngine;

public class EnemyVerticalMovement : BaseEnemyMovement
{
    private bool movingUp = true;
    private bool hasPlayedFirstSound = false; // Để phát âm thanh ngay lần đầu

    protected override void Start()
    {
        base.Start();
        footstepTimer = 0f; // Đảm bảo timer bắt đầu từ 0
        hasPlayedFirstSound = false;
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
}
