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
