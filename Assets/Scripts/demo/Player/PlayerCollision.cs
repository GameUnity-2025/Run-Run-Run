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
            
            // Phát âm thanh ăn gem
            PlayGemCollectSound();
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
        // Kiểm tra rơi xuống nước (có tag "Water" hoặc layer "Water")
        else if (collision.CompareTag("Water") || collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            PlayWaterSplashSound();
            gameManager.GameOver();
        }
    }

    private void PlayGemCollectSound()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.gemCollectSound != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.gemCollectSound, SoundManager.Instance.defaultSFXVolume);
        }
    }

    private void PlayWaterSplashSound()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.waterSplashSound != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.waterSplashSound, SoundManager.Instance.defaultSFXVolume);
        }
    }
}