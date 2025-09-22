using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StartButton : MonoBehaviour
{

    [SerializeField] private List<Text> uiText;
    [SerializeField] private float fadeSpeed = 1.5f;
    private bool fadingOut = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < uiText.Count; i++)
        {
            Color color = uiText[i].color;

            float alphaChange = fadeSpeed * Time.deltaTime;

            if (fadingOut)
            {
                color.a -= alphaChange;
                if (color.a <= 0f)
                {
                    color.a = 0f;
                    fadingOut = false;
                }
            }
            else
            {
                color.a += alphaChange;
                if (color.a >= 1f)
                {
                    color.a = 1f;
                    fadingOut = true;
                }
            }

            uiText[i].color = color;
        }
        

    }
    
}