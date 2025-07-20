using UnityEngine;
using UnityEngine.SceneManagement;


public class ScenesRetry : MonoBehaviour
{
    private string currentSceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressRetryButton()
    {
        SceneManager.LoadScene(currentSceneName);
    }
}
