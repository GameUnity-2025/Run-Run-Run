using UnityEngine;

public class PlayerOutOfViewKill : MonoBehaviour
{
    [Header("Die when falling below camera view")]
    public float extraMargin = 1.0f; // thêm biên an toàn dưới đáy màn hình
    public bool onlyBelow = true;     // chỉ chết khi rơi xuống dưới (không xét trái/phải/trên)

    private GameManager gameManager;
    private Camera mainCam;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
        if (mainCam == null)
            mainCam = Camera.main;
        if (mainCam == null || gameManager == null)
            return;

        Vector3 camPos = mainCam.transform.position;
        Vector3 playerPos = transform.position;

        if (mainCam.orthographic)
        {
            float bottom = camPos.y - mainCam.orthographicSize - extraMargin;
            if (playerPos.y < bottom)
            {
                gameManager.GameOver();
                return;
            }

            if (!onlyBelow)
            {
                float top = camPos.y + mainCam.orthographicSize + extraMargin;
                float halfWidth = mainCam.orthographicSize * mainCam.aspect;
                float left = camPos.x - halfWidth - extraMargin;
                float right = camPos.x + halfWidth + extraMargin;
                if (playerPos.y > top || playerPos.x < left || playerPos.x > right)
                {
                    gameManager.GameOver();
                    return;
                }
            }
        }
        else
        {
            // Fallback đơn giản cho camera perspective: xét rơi xuống dưới một ngưỡng tương đối so với camera
            if (playerPos.y < camPos.y - 20f)
            {
                gameManager.GameOver();
            }
        }
    }
}




