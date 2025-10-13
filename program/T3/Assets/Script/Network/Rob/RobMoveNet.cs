using UnityEngine;

public class RobMoveNet : RobMove
{
    private bool IsServer => GetComponent<RobBaseNet>()?.Object.HasStateAuthority ?? false;
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
