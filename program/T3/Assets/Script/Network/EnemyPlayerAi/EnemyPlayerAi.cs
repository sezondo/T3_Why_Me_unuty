using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPlayerAi : NetworkBehaviour
{
    [SerializeField] RobReadyEnemyData[] robNetworkUnitDatas;
    [SerializeField] float minInterval = 1f, maxInterval = 10f;
    [SerializeField] float maxCost = 20f, regenPerSec = 1f;
    [SerializeField] Collider enemySpawnArea;

    float currentCost;
    float nextSpawnTime;

    public override void Spawned()
    {
        if (!Runner.IsServer)
        {
            return;
        }
        currentCost = maxCost * 0.5f;
        ScheduleNextSpawn();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer)
        {
            return;
        }

        if (enemySpawnArea == null)
        {
            return;
        }

        currentCost = Mathf.Min(maxCost, currentCost + regenPerSec * Runner.DeltaTime);
        if (Runner.SimulationTime >= nextSpawnTime) {
            TrySpawnUnit();
            ScheduleNextSpawn();
        }
    }

    void TrySpawnUnit()
    {
        var candidates = robNetworkUnitDatas.Where(u => u.cost <= currentCost).ToList();
        if (candidates.Count == 0) return;

        int candidateslength = Random.Range(0, candidates.Count);
        var chosen = candidates[candidateslength];

        Runner.Spawn(chosen.RobRedayPrefab, GetRandomSpawnPosition());

        currentCost -= chosen.cost;
    }

    Vector3 GetRandomSpawnPosition()
    {
        var bounds = enemySpawnArea.bounds;

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.center.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (NavMesh.SamplePosition(randomPoint, out var hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        Vector3 defaultPoint = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);

        return defaultPoint;
    }

    void ScheduleNextSpawn()
    {
        nextSpawnTime = Runner.SimulationTime + Random.Range(minInterval, maxInterval);

    }
}
