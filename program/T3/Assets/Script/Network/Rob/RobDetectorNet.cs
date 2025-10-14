using UnityEngine;
using System.Collections;

public class RobDetectorNet : RobDetector
{
    private bool IsServer => robBaseNetCash != null && robBaseNetCash.Object.HasStateAuthority;
    private RobBaseNet robBaseNetCash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        robBaseNetCash = GetComponent<RobBaseNet>();

    }
    public override void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitUntil(() => robBaseNetCash.Object != null);

        if (!IsServer)
        {
            StopAllCoroutines();
            this.enabled = false;
            yield break;
        }
        

        base.Start();
    }

}
