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

        // Lấy scene hiện tại
        string currentScene = SceneManager.GetActiveScene().name;

        // Load lại đúng scene đó
        SceneManager.LoadScene(currentScene);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void NextLevel()
    {
        // Lấy tên scene hiện tại
        string currentScene = SceneManager.GetActiveScene().name;
        string nextScene = "";

        // 🟢 Nếu đang ở level đầu tiên (tên là "Game")
        if (currentScene == "Game")
        {
            nextScene = "Level2";
        }
        else if (currentScene.StartsWith("Level"))
        {
            // 🟢 Cắt phần số phía sau tên scene
            string levelNumberStr = currentScene.Substring(5); // bỏ "Level"
            if (int.TryParse(levelNumberStr, out int levelNumber))
            {
                nextScene = "Level" + (levelNumber + 1);
            }
            else
            {
                Debug.LogWarning("Tên scene hiện tại không đúng định dạng LevelX!");
                return;
            }
        }
        else
        {
            Debug.LogWarning("Scene hiện tại không nằm trong hệ thống level!");
            return;
        }

        // 🟢 Kiểm tra xem level kế có tồn tại không
        if (Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextScene);
            Debug.Log($"Loading next level: {nextScene}");
        }
        else
        {
            // 🔚 Nếu không có level kế → quay lại Menu
            Debug.Log("Không có level tiếp theo → quay lại Menu.");
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        }
    }


    public bool IsGameWon()
    {
        return isGameWon;
    }
}
