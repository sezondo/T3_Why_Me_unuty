using UnityEngine;

public class RobDragAndDropNet : RobDragAndDrop
{
    public override void Drag()
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
        {   // 코스트 초과일 경우
            if (CostManagerNet.instance.currentCost < robBaseReady.robRedayData.cost)
            {
                ReadyManager.instance.StartPopupCostOverrun();
                ReadyManager.instance.useButton = false;
                Destroy(gameObject);

                return;

            }
            // Ground가 아닌 이상한데 둘 경우
            if (!Physics.Raycast(ray, 100f, LayerMask.GetMask("Ground")) && !(CostManagerNet.instance.currentCost < robBaseReady.robRedayData.cost))
            {
                ReadyManager.instance.useButton = false;
                Destroy(gameObject);
                return;
            }

            ConfirmPlacement(hit.point);
        }
    }
    
    protected override void ConfirmPlacement(Vector3 position)
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("Ally"));

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // 자기 자신 제외
            {
                Debug.Log("겹침");
                ReadyManager.instance.StartPopup(); //이건 한번 인게임 테스트 해보면서 수정
                return;
            }
        }
        */

        if (robBaseReady.readyState != ReadyUnitState.Readyed)
        {
            ReadyManager.instance.DorpAudio();
            ReadyManager.instance.useButton = false;
            robBaseReady.ChangeState(ReadyUnitState.Readyed);

        }

        
    }

}
