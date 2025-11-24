using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    public float delayTime = 2.5f;

    private void Start()
    {
        Invoke("GoNext", delayTime);
    }

    void GoNext()
    {
        SceneManager.LoadScene("Menu");
    }
}