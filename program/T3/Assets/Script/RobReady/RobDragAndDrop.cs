using UnityEngine;
using System.Collections;

public class RobDragAndDrop : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    private GameObject currentPreview;
    private RobBaseReady robBaseReady;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPreview = this.gameObject;
        robBaseReady = GetComponent<RobBaseReady>();
    }

    // Update is called once per frame
    void Update()
    {
        Drag();
    }
    public void Drag()
    {
        if (robBaseReady.readyState == ReadyUnitState.Readyed) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            currentPreview.transform.position = hit.point;
            if (robBaseReady.readyState != ReadyUnitState.Readying)
            {
                robBaseReady.ChangeState(ReadyUnitState.Readying);
            }
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 놓음
        {
            if (ReadyManager.instance.currentCost + robBaseReady.robRedayData.cost > ReadyManager.instance.levelData.Cost)
            {
                ReadyManager.instance.StartPopupCostOverrun();
                ReadyManager.instance.useButton = false;
                Destroy(gameObject);
                return;

            }

            if (!Physics.Raycast(ray, 100f, LayerMask.GetMask("Ground")) && !(ReadyManager.instance.currentCost + robBaseReady.robRedayData.cost > ReadyManager.instance.levelData.Cost))
            {
                ReadyManager.instance.useButton = false;
                Destroy(gameObject);
                return;
            }

            ConfirmPlacement(hit.point);
        }

    }
    void ConfirmPlacement(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("Ally"));

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // 자기 자신 제외
            {
                Debug.Log("겹침");
                ReadyManager.instance.StartPopup();
                return;
            }
        }

        if (robBaseReady.readyState != ReadyUnitState.Readyed)
        {
            ReadyManager.instance.useButton = false;
            robBaseReady.ChangeState(ReadyUnitState.Readyed);
        }

        ReadyManager.instance.AddCost(robBaseReady.robRedayData.cost);
        ReadyManager.instance.currentPreviews.Add(this.gameObject);

    }

    




    
}
