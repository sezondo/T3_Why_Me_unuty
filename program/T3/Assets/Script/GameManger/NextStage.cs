using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class NextStage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadNextStage()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        Match match = Regex.Match(currentScene, @"GameScene_Level(\d+)");
        if (match.Success)
        {
            int currentStage = int.Parse(match.Groups[1].Value);
            int nextStage = currentStage + 1;

            string nextSceneName = $"GameScene_Level{nextStage}";

            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("게임끝");
                //SceneManager.LoadScene("MainScene"); 
            }
        }
        else
        {
            Debug.LogError("현재 씬에서 추출 불가");
        }
    }
}
