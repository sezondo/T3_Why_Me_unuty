using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkRobSpawnManmeger : NetworkBehaviour
{
    public static NetworkRobSpawnManmeger instance { get; private set; }

    [SerializeField] private List<RobNetworkUnitData> spawnableUnits = new();
    [SerializeField] private Vector3 defaultPreloadPosition = new Vector3(1000f, 0f, 0f);

    private readonly Dictionary<RobNetworkUnitData, byte> lookup = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        BuildLookup();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void BuildLookup()
    {
        lookup.Clear();

        for (byte i = 0; i < spawnableUnits.Count && i < byte.MaxValue; i++)
        {
            RobNetworkUnitData data = spawnableUnits[i];
            if (data == null || lookup.ContainsKey(data))
            {
                continue;
            }

            lookup.Add(data, i);
        }
    }

    /// <summary>
    /// 요청한 플레이어에게 소유권을 부여하고 실제 전투 유닛을 스폰합니다.
    /// </summary>
    public NetworkObject RequestRealUnitSpawn(RobNetworkUnitData unitData, Vector3 preloadPosition, Quaternion rotation, PlayerRef owner)
    {
        if (unitData == null)
        {
            Debug.LogWarning("[NetworkSpawnManmeger] unitData is null.");
            return null;
        }

        if (Matchmaker.Runner == null)
        {
            Debug.LogWarning("[NetworkSpawnManmeger] Runner not ready.");
            return null;
        }

        if (!lookup.TryGetValue(unitData, out byte id))
        {
            Debug.LogWarning("[NetworkSpawnManmeger] unitData not registered in list.");
            return null;
        }

        if (Object != null && Object.HasStateAuthority)
        {
            return SpawnRealUnitInternal(id, preloadPosition, rotation, owner);
        }

        RPC_RequestRealUnitSpawn(owner, id, preloadPosition, rotation);
        return null;
    }

    public NetworkObject SpawnRealUnitImmediate(RobNetworkUnitData unitData, PlayerRef owner)
    {
        return RequestRealUnitSpawn(unitData, defaultPreloadPosition, Quaternion.identity, owner);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestRealUnitSpawn(PlayerRef owner, byte dataId, Vector3 preloadPosition, Quaternion rotation, RpcInfo info = default)
    {
        SpawnRealUnitInternal(dataId, preloadPosition, rotation, owner);
    }

    private NetworkObject SpawnRealUnitInternal(byte dataId, Vector3 preloadPosition, Quaternion rotation, PlayerRef owner)
    {
        if (Matchmaker.Runner == null)
        {
            return null;
        }

        if (dataId >= spawnableUnits.Count)
        {
            Debug.LogWarning("[NetworkSpawnManmeger] Invalid data id.");
            return null;
        }

        RobNetworkUnitData data = spawnableUnits[dataId];
        if (data == null || data.RobPrefab == null)
        {
            Debug.LogWarning("[NetworkSpawnManmeger] Prefab missing for data id.");
            return null;
        }

        return Matchmaker.Runner.Spawn(data.RobPrefab, preloadPosition, rotation, owner);
    }
}
