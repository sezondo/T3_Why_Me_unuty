using UnityEngine;
using System.Collections;

public class RobDetectorNet : RobDetector
{

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            StopAllCoroutines();
            this.enabled = false;
            return;
        }

        base.Start();
    }

    public override void Start() { }

}
