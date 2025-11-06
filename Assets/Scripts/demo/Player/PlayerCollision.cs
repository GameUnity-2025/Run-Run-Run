using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;
    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gem"))
        {
            Destroy(collision.gameObject);
			gameManager.AddScore(1);
			GemsManager.AddGems(1);
        }
        else if (collision.CompareTag("Trap"))
        {
            gameManager.GameOver();
        }
        else if (collision.CompareTag("Enemy"))
        {
            gameManager.GameOver();
        }
        // Die on touching ForeGround layer (no tag required)
        else if (collision.gameObject.layer == LayerMask.NameToLayer("ForeGround"))
        {
            gameManager.GameOver();
        }
        else if (collision.CompareTag("Goal"))
        {
            gameManager.GameWin();
        }
    }
}