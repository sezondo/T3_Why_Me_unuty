using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public GameObject selectStage;
    public GameObject robGameObject;
    public GameObject selectedStage;
    private Carousel carousel;
    private int carouseSelet;
    public GameObject[] StageContor;
    //public String[] sceneName; //이게 오히려 유지보수가 힘들듯
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carousel = selectedStage.GetComponent<Carousel>();

        
        for (int i = 0; i < PlayerManager.instance.clearNumber; i++)
        {
            StageContor[i].SetActive(false);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(carousel.currentSelected);
        Debug.Log(PlayerManager.instance.clearNumber);
    }
    public void PressStart()
    {
        selectStage.SetActive(true);
        robGameObject.SetActive(false);
    }

    public void StageStart()
    {
        carouseSelet = carousel.currentSelected + 1;
        if (carouseSelet <= PlayerManager.instance.clearNumber + 1)
        {
            switch (carouseSelet)
            {
                case 1:
                    SceneManager.LoadScene("GameScene_Level1");
                    break;
                case 2:
                    SceneManager.LoadScene("GameScene_Level2");
                    break;
                case 3:
                    SceneManager.LoadScene("GameScene_Level3");
                    break;
                case 4:
                    SceneManager.LoadScene("GameScene_Level4");
                    break;
                case 5:
                    SceneManager.LoadScene("GameScene_Level5");
                    break;
                case 6:
                    SceneManager.LoadScene("GameScene_Level6");
                    break;
                case 7:
                    SceneManager.LoadScene("GameScene_Level7");
                    break;
                default:
                    break;
            }
        }
    }
    
}
