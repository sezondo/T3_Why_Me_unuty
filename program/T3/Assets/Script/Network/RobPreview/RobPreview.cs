using Fusion;
using UnityEngine;

public class RobPreview : NetworkBehaviour
{
    protected RobBaseReady robBaseReady;
    private NetworkObject networkObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robBaseReady = GetComponent<RobBaseReady>();
    }

    // Update is called once per frame
    void Update()
    {
        if (robBaseReady.readyState == ReadyUnitState.Readyed)
        {
            networkObject = Runner.Spawn(robBaseReady.robRedayData.RobRedayPrefab, transform.position);
        }

        if (networkObject != null)
        {
            Destroy(gameObject);
        }
    }

    
}
