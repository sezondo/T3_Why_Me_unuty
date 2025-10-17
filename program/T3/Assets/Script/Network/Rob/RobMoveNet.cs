using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class RobMoveNet : RobMove
{
    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            var agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
            }
            StopAllCoroutines();
            this.enabled = false;
            return;
        }

        base.Start();
    }

    public override void Start() { }

    public override void Update()
    {}

    public override void FixedUpdateNetwork()
    {
        base.Update();
    }
}
