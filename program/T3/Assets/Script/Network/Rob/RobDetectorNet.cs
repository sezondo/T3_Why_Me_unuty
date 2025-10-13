using UnityEngine;

public class RobDetectorNet : RobDetector
{
    private bool IsServer => GetComponent<RobBaseNet>()?.Object.HasStateAuthority ?? false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        if (!IsServer)
        {
            StopAllCoroutines();
            this.enabled = false;
            return;
        }

        base.Start();
    }

}
