using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LocalToNetworkSpawnManmeger : NetworkBehaviour
{
    public static LocalToNetworkSpawnManmeger instance { get; private set; }

    [SerializeField] private List<RobReadyData> spawnableUnits = new();

    private readonly Dictionary<RobReadyData, byte> dataToId = new();

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
        dataToId.Clear();

        for (byte i = 0; i < spawnableUnits.Count && i < byte.MaxValue; i++)
        {
            var data = spawnableUnits[i];
            if (data == null || dataToId.ContainsKey(data))
            {
                continue;
            }

            dataToId.Add(data, i);
        }
    }

    public void RequestReadyUnitSpawn(RobReadyData unitData, Vector3 position, Quaternion rotation)
    {
        if (unitData == null)
        {
            Debug.LogWarning("[SpawnManmeger] RequestReadyUnitSpawn called with null data.");
            return;
        }

        if (Matchmaker.Runner == null)
        {
            Debug.LogWarning("[SpawnManmeger] Runner not ready, cannot spawn.");
            return;
        }

        if (!dataToId.TryGetValue(unitData, out var dataId))
        {
            Debug.LogWarning("[SpawnManmeger] Unit data not registered, cannot spawn.");
            return;
        }

        var requester = Matchmaker.Runner.LocalPlayer;

        if (Object != null && Object.HasStateAuthority)
        {
            SpawnReadyUnit(dataId, position, rotation, requester);
        }
        else
        {
            RPC_RequestReadyUnitSpawn(requester, dataId, position, rotation);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)] // 누구나 호출할수 있지만 실행은 서버만 해라
    private void RPC_RequestReadyUnitSpawn(PlayerRef requester, byte dataId, Vector3 position, Quaternion rotation, RpcInfo info = default)
    {
        SpawnReadyUnit(dataId, position, rotation, requester);
    }

    private void SpawnReadyUnit(byte dataId, Vector3 position, Quaternion rotation, PlayerRef owner)
    {
        if (Matchmaker.Runner == null)
        {
            return;
        }

        if (dataId >= spawnableUnits.Count)
        {
            Debug.LogWarning("[SpawnManmeger] Invalid data id.");
            return;
        }

        RobReadyData data = spawnableUnits[dataId];

        if (data == null || data.RobRedayPrefab == null)
        {
            Debug.LogWarning("[SpawnManmeger] Missing prefab for data id.");
            return;
        }

        Matchmaker.Runner.Spawn(data.RobRedayPrefab, position, rotation, owner);
    }
}
