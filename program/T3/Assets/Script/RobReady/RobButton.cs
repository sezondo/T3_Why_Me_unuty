using UnityEngine;

public class RobButton : MonoBehaviour
{
    private GameObject previewUnitPrefab01;
    private GameObject previewUnitPrefab02;
    private GameObject previewUnitPrefab03;
    private GameObject previewUnitPrefab04;
    private GameObject previewUnitPrefab05;
    private GameObject previewUnitPrefab06;
    private GameObject previewUnitPrefab07;
    private GameObject previewUnitPrefab08;
    private GameObject previewUnitPrefab012;
    public RobRedayData robRedayData01;
    public RobRedayData robRedayData02;
    public RobRedayData robRedayData03;
    public RobRedayData robRedayData04;
    public RobRedayData robRedayData05;
    public RobRedayData robRedayData06;
    public RobRedayData robRedayData07;
    public RobRedayData robRedayData08;
    public RobRedayData robRedayData12;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previewUnitPrefab01 = robRedayData01.RobRedayPrefab;
        previewUnitPrefab02 = robRedayData02.RobRedayPrefab;
        previewUnitPrefab03 = robRedayData03.RobRedayPrefab;
        previewUnitPrefab04 = robRedayData04.RobRedayPrefab;
        previewUnitPrefab05 = robRedayData05.RobRedayPrefab;
        previewUnitPrefab06 = robRedayData06.RobRedayPrefab;
        previewUnitPrefab07 = robRedayData07.RobRedayPrefab;
        previewUnitPrefab08 = robRedayData08.RobRedayPrefab;
        previewUnitPrefab012 = robRedayData12.RobRedayPrefab;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartDragRob01()
    {
        Drop(previewUnitPrefab01);
    }
    public void StartDragRob02()
    {
        Drop(previewUnitPrefab02);
    }
    public void StartDragRob03()
    {
        Drop(previewUnitPrefab03);
    }
    public void StartDragRob04()
    {
        Drop(previewUnitPrefab04);
    }
    public void StartDragRob05()
    {
        Drop(previewUnitPrefab05);
    }
    public void StartDragRob06()
    {
        Drop(previewUnitPrefab06);
    }
    public void StartDragRob07()
    {
        Drop(previewUnitPrefab07);
    }
    public void StartDragRob08()
    {
        Drop(previewUnitPrefab08);
    }
    public void StartDragRob12()
    {
        Drop(previewUnitPrefab012);
    }

    private void Drop(GameObject previewUnitPrefab)
    {
        if (!ReadyManager.instance.useButton)
        {
            ReadyManager.instance.useButton = true;
            Instantiate(previewUnitPrefab);
        }
        
    }

    
}
