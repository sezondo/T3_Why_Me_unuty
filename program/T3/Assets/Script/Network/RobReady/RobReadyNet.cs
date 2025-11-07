using Fusion;
using UnityEngine;

public class RobReadyNet : NetworkBehaviour
{
    public RobNetworkUnitData robNetworkUnitData;
    private TickTimer Tick { get; set; }
    private TickTimer _Tick { get; set; }
    private bool stopFlag;
    private NetworkObject realUnit;

    private NetworkTransform networkTransform;
    private Vector3 vector3;

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            Tick = TickTimer.CreateFromSeconds(Runner, robNetworkUnitData.spongeWaitingTime);
            vector3 = transform.position;
            //realUnit = Runner.Spawn(robNetworkUnitData.RobPrefab, vector3); //  실제 유닛 미리 생성
            //networkTransform = realUnit.GetComponent<NetworkTransform>();
        }
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer)
        {
            return;
        }
        if (!stopFlag && Tick.Expired(Runner))
        {
            stopFlag = true;

            NetworkRobSpawnManmeger.instance.RequestRealUnitSpawn(robNetworkUnitData, transform.position, transform.rotation, Object.InputAuthority);
            
            _Tick = TickTimer.CreateFromSeconds(Runner, 0.05f);
        }
        
        if (_Tick.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
