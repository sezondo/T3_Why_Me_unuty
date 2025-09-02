using UnityEngine;

public class RobBaseReady : MonoBehaviour
{
    public ReadyUnitState readyState;
    public RobReadyData robRedayData;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ReadyManager.instance.readyManagerState == ReadyManagerState.Start)
        {
            Destroy(gameObject);
        }
    }
    public void ChangeState(ReadyUnitState unitState)
    {
        if ( this.readyState == unitState) return;

        this.readyState = unitState;
    }
}
