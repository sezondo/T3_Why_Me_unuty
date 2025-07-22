using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using Ricimi;


public struct ReadyCompletionData
{
    public Vector3 preFabDataVector3;
    public GameObject preFabData;
}

public class ReadyManager : MonoBehaviour
{
    public List<GameObject> currentPreviews = new List<GameObject>();
    private ReadyCompletionData readyCompletionData;
    public bool useButton;
    public static ReadyManager instance;
    public Image popupImage;
    public Image popupImageCostOverrun;
    public CanvasGroup popupGroup;
    public CanvasGroup popupCostOverrunGroup;
    public LevelData levelData;
    [SerializeField] private LayerMask allyLayerMask;
    public ReadyManagerState readyManagerState;
    public int currentCost;
    public Text currentCostText;
    [SerializeField] private AudioClip popupClip;
    [SerializeField] private AudioClip dropClip;

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

        readyCompletionData = new ReadyCompletionData();
        readyManagerState = ReadyManagerState.Ready;
    }
    public void StartPopup()
    {
        if (!popupImage.enabled)
        {
            PopupAudio();
            StartCoroutine(Popup());
        }

    }


    // Update is called once per frame
    void Update()
    {
        currentCostText.text = "Current Cost : " + currentCost.ToString();
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

    public void GameStart()
    {

        List<ReadyCompletionData> readyingPositions = new List<ReadyCompletionData>();

        Collider[] colliders = Physics.OverlapSphere(Vector3.zero, 1000f, allyLayerMask);
        HashSet<Transform> chekedParents = new HashSet<Transform>();

        foreach (Collider col in colliders)
        {
            Debug.Log("유닛 수: " + readyingPositions.Count);

            Transform parent = col.transform.root;

            if (chekedParents.Contains(parent))
            {
                continue;
            }

            chekedParents.Add(parent);

            RobBaseReady readyComp = parent.GetComponent<RobBaseReady>();

            if (readyComp != null && readyComp.readyState == ReadyUnitState.Readyed)
            {
                readyCompletionData.preFabData = readyComp.robRedayData.RobPrefab;
                readyCompletionData.preFabDataVector3 = parent.position;
                Debug.Log("readyingPositions add 잘됨 ");
                readyingPositions.Add(readyCompletionData);
            }
        }

        foreach (ReadyCompletionData rcd in readyingPositions)
        {
            Instantiate(rcd.preFabData, rcd.preFabDataVector3, Quaternion.identity);
        }

        readyManagerState = ReadyManagerState.Start;


    }

    public void CancelAllPlacement()
    {
        currentCost = 0;
        foreach (var preview in currentPreviews)
        {
            RobBaseReady baseReady = preview.GetComponent<RobBaseReady>();
            if (baseReady != null)
            {
                Destroy(baseReady.gameObject);
            }
        }
        currentPreviews.Clear();
    }

    public void AddCost(int cost)
    {
        currentCost += cost;
    }

    public void StartPopupCostOverrun()
    {
        if (!popupImageCostOverrun.enabled)
        {
            PopupAudio();
            StartCoroutine(PopupCostOverrun());
        }
    }
    public IEnumerator PopupCostOverrun()
    {
        popupImageCostOverrun.enabled = true;
        popupCostOverrunGroup.alpha = 0f;
        popupCostOverrunGroup.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(3f);


        popupCostOverrunGroup.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(1f);
        popupImageCostOverrun.enabled = false;

    }

    public void PopupAudio()
    {
        SoundManager.instance.PlaySFXUI(popupClip);
    }

    public void DorpAudio()
    {
        SoundManager.instance.PlaySFXUI(dropClip);

    }
}
