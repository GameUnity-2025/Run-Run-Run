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
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play_Coin();
        }
        else if (collision.CompareTag("Trap"))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play_Fail();
            gameManager.GameOver();
        }
        else if (collision.CompareTag("Enemy"))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play_Fail();

            gameManager.GameOver();
        }
        // Die on touching ForeGround layer (no tag required)
        else if (collision.gameObject.layer == LayerMask.NameToLayer("ForeGround"))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play_Fail();

            gameManager.GameOver();
        }
        else if (collision.CompareTag("Goal"))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play_Success();

            gameManager.GameWin();
        }
        // Kiểm tra rơi xuống nước (kiểm tra layer "Water")
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play_Fail();

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