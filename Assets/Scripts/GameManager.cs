using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverUI;
    private bool isGameOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore();
        gameOverUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore(int points)
    {
        score += points; 
        UpdateScore();
    }
    public void UpdateScore() { 
        scoreText.text = score.ToString();
    }
    public void GameOver() 
    {
        isGameOver = true;
        score = 0;
        Time.timeScale = 0f; // Pause the game
        gameOverUI.SetActive(true);
    }
    public void RestartGame() 
    {
        isGameOver = false;
        score = 0;
        UpdateScore();
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene("Game");
        
    }
    public bool IsGameOver() 
    {
        return isGameOver;
    }
}
