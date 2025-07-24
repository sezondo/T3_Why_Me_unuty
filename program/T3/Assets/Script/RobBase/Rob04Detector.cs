using UnityEngine;

public class Rob04Detector : RobDetector
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
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
