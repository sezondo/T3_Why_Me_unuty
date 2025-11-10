using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobReadyNet : NetworkBehaviour
{
    public RobNetworkUnitData robNetworkUnitData;
    [HideInInspector, Networked] public TickTimer Tick { get; private set; }
    private TickTimer _Tick { get; set; }
    private bool stopFlag;
    private NetworkObject realUnit;
    private NetworkTransform networkTransform;
    public bool IsSpawned { get; private set; } 

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            Tick = TickTimer.CreateFromSeconds(Runner, robNetworkUnitData.spongeWaitingTime);

            //realUnit = Runner.Spawn(robNetworkUnitData.RobPrefab, vector3); //  실제 유닛 미리 생성
            //networkTransform = realUnit.GetComponent<NetworkTransform>();
        }
        IsSpawned = true;
        
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

            realUnit = NetworkRobSpawnManmeger.instance.RequestRealUnitSpawn(robNetworkUnitData, transform.position, transform.rotation, Object.InputAuthority);
            var agent = realUnit.GetComponent<NavMeshAgent>();
            agent.Warp(transform.position);
            
            _Tick = TickTimer.CreateFromSeconds(Runner, 0.05f);
        }
        
        if (_Tick.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
