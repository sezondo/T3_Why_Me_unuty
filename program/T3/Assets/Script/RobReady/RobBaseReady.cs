using UnityEngine;

public class RobBaseReady : MonoBehaviour
{
    public ReadyState readyState;
    public RobRedayData robRedayData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeState(ReadyState unitState)
    {
        if ( this.readyState == unitState) return;

        this.readyState = unitState;
    }
}
