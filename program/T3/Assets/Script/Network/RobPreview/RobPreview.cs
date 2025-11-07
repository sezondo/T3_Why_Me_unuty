using Fusion;
using UnityEngine;

public class RobPreview : MonoBehaviour
{
    protected RobBaseReady robBaseReady;

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
            LocalToNetworkSpawnManmeger.instance.RequestReadyUnitSpawn(robBaseReady.robRedayData, transform.position, transform.rotation);

            Destroy(gameObject);
        }

    }

    
}
