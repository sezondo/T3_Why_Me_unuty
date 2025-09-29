using Fusion;
using UnityEngine;

public class NetDebug : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef unitPrefab; // Prefab Table에 등록된 프리팹을 드래그
    [SerializeField] private Vector3 spawnPos = new Vector3(0, 1, 0);

    public void HostDebugSpawn()
    {
        if (!Object.HasStateAuthority) return; // Host만 실행
        Runner.Spawn(unitPrefab, spawnPos, Quaternion.identity, PlayerRef.None);
        Debug.Log("[Host] Spawned test unit");
    }
}
