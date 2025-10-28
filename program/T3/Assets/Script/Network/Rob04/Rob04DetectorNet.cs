using UnityEngine;

public class Rob04DetectorNet : RobDetectorNet
{
    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            StopAllCoroutines();
            this.enabled = false;
            return;
        }

        robBase = GetComponent<RobBase>();
        robMove = GetComponent<RobMove>();
        StartCoroutine(Detector());

        if (robBase.data.faction == FactionType.Ally)
        {
            layerMask = LayerMask.GetMask("Enemy");
        }
        else if (robBase.data.faction == FactionType.Enemy)
        {
            layerMask = LayerMask.GetMask("Ally");
        }
    }
}
