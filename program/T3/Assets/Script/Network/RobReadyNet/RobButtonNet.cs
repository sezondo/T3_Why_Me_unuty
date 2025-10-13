using UnityEngine;
using UnityEngine.UI;
using Fusion;
using static Unity.Collections.Unicode;

public class RobButtonNet : RobButton
{
    public override void Drop(GameObject previewUnitPrefab)
    {
        Vector3 vector3 = new Vector3(0, 100, 0);
        Matchmaker.Runner.Spawn(previewUnitPrefab, vector3,Quaternion.identity);
    }


}
 