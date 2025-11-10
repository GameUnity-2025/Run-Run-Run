using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeController : MonoBehaviour
{
    [SerializeField] int maxPage;
    int currentPage = 0;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;

    [SerializeField] float tweenTime = 0.5f;
    [SerializeField] LeanTweenType tweenType;

    [Header("Scene Names")]
    [SerializeField] string menuSceneName = "MainMenu";

    bool isMoving = false; 

    private void Awake()
    {
        currentPage = 1;
        targetPos = levelPagesRect.localPosition;
    }

    public void Next()
    {
        if (isMoving) return; 
        if (currentPage < maxPage)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play_Button();
            }

            currentPage++;
            targetPos = levelPagesRect.localPosition - pageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if (isMoving) return; 
        if (currentPage > 1)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play_Button();
            }

            currentPage--;
            targetPos = levelPagesRect.localPosition + pageStep;
            MovePage();
        }
    }

    void MovePage()
    {
        isMoving = true; 
        levelPagesRect.LeanMoveLocal(targetPos, tweenTime)
            .setEase(tweenType)
            .setOnComplete(() =>
            {
                isMoving = false; 
            });
    }

    public void BackToMenu()
    {
        if (isMoving) return;

        // Try multiple possible scene names
        string[] possibleNames = { menuSceneName, "MainMenu", "Main Menu", "Menu" };

        foreach (string name in possibleNames)
        {
            if (SceneExists(name))
            {
                Debug.Log($"Loading menu scene: {name}");
                SceneManager.LoadScene(name);
                return;
            }
        }

        // If no scene found, try loading by index (usually scene 0 is main menu)
        Debug.LogWarning("Menu scene not found by name, trying to load scene 0");
        SceneManager.LoadScene(0);
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameInBuild == sceneName)
                return true;
        }
        return false;
    }

    public void PlayCurrentLevel()
    {
        Debug.Log("Play level " + currentPage);

        string sceneName;
        if (currentPage == 1)
        {
            sceneName = "Game";
        }
        else
        {
            
            sceneName = "Level" + currentPage;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' chưa được thêm vào Build Settings!");
        }
    }
}
