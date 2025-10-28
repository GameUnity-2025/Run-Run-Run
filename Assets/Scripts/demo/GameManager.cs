using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private GameObject GameWinUi;
    private bool isGameWon = false;
    private bool isGameOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreUI();
        gameOverUi.SetActive(false);
        GameWinUi.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddScore(int points)
    {
        if (!isGameOver||!isGameWon)
        {
            score += points;
            UpdateScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            if (gameOverUi != null)
            {
                gameOverUi.SetActive(true);
            }
            Debug.Log("Game Over! Final Score: " + score);
        }
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    public void GameWin()
    {
        if (!isGameWon)
        {
            isGameWon = true;
            Time.timeScale = 0;
            if (GameWinUi != null)
            {
                GameWinUi.SetActive(true);
            }
            Debug.Log("You Win! Final Score: " + score);
        }
    }

    public void RestartGame()
    {
        isGameOver = false;
        score = 0;
        UpdateScoreUI();
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsGameWon()
    {
        return isGameWon;
    }
}
