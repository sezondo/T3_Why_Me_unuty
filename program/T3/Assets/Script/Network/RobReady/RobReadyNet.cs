using Fusion;
using UnityEngine;

public class RobReadyNet : NetworkBehaviour
{
    RobNetworkUnitData robNetworkUnitData;
    private TickTimer Tick { get; set; }
    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            Tick = TickTimer.CreateFromSeconds(Runner, robNetworkUnitData.spongeWaitingTime);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void FixedUpdateNetwork()
    {
        if(Tick.Expired(Runner))
        {
            Runner.Spawn(robNetworkUnitData.RobPrefab, transform.position);
            
        }
    }
}
