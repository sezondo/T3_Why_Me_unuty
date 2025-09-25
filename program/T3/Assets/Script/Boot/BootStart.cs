using UnityEngine;
using UnityEngine.SceneManagement;
public class BootStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene("MainScene");
    }

    
}
