using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;



public class ReadyManager : MonoBehaviour
{
    public bool useButton;
    public static ReadyManager instance;
    public Image popupImage;
    public CanvasGroup popupGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우

            // 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject);
        }
    }
    public void StartPopup()
    {
        if (!popupImage.enabled)
        {
            StartCoroutine(Popup());
        }
        
    }


    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Popup()
    {
        popupImage.enabled = true;
        popupGroup.alpha = 0f;
        popupGroup.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(3f);
        popupGroup.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(1f);
        popupImage.enabled = false;
    }
}
