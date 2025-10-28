using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gem")){
            Destroy(collision.gameObject);
            gameManager.IncreaseScore(1);
        } else if (collision.CompareTag("Trap")){
            gameManager.GameOver();
        }
    }
}
