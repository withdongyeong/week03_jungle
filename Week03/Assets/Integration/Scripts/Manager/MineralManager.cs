using UnityEngine;
using System.Collections.Generic;

public class MineralManager : MonoBehaviour
{
    public GameObject mineral1Prefab;
    public GameObject mineral2Prefab;
    public GameObject mineral3Prefab;

    public PoolKey mineral1Key = PoolKey.Mineral_1;
    public PoolKey mineral2Key = PoolKey.Mineral_2;
    public PoolKey mineral3Key = PoolKey.Mineral_3;

    private float spawnTimer = 0f;
    private List<GameObject> activeMinerals = new();

    private int MaxCount => GlobalSettings.Instance.maxMineralCount;
    private float SpawnInterval => GlobalSettings.Instance.mineralSpawnInterval;
    private float SpawnRange => GlobalSettings.Instance.mineralSpawnRange;

    private float Prob1 => GlobalSettings.Instance.mineral1Probability;
    private float Prob2 => GlobalSettings.Instance.mineral2Probability;
    private float Prob3 => GlobalSettings.Instance.mineral3Probability;

    private void Start()
    {
        ObjectPoolManager.Instance.CreatePool(mineral1Key, mineral1Prefab, 10);
        ObjectPoolManager.Instance.CreatePool(mineral2Key, mineral2Prefab, 10);
        ObjectPoolManager.Instance.CreatePool(mineral3Key, mineral3Prefab, 10);
    }

    private void Update()
    {
        if (activeMinerals.Count >= MaxCount) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= SpawnInterval)
        {
            TrySpawnMineral();
            spawnTimer = 0f;
        }
    }

    private void TrySpawnMineral()
    {
        PoolKey keyToSpawn = GetRandomMineralKey();
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject mineral = ObjectPoolManager.Instance.SpawnFromPool(keyToSpawn, spawnPos, Quaternion.identity);
        if (mineral == null) return;

        activeMinerals.Add(mineral);
    }

    private PoolKey GetRandomMineralKey()
    {
        float rand = Random.value;
        if (rand < Prob3) return mineral3Key;
        else if (rand < Prob2 + Prob3) return mineral2Key;
        else return mineral1Key;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var player = HW_PlayerStateController.Instance;
        if (player == null) return Vector3.zero;

        Vector3 basePos = player.transform.position;
        float x = Random.Range(-SpawnRange, SpawnRange);
        float z = Random.Range(-SpawnRange, SpawnRange);
        float y = 1f;

        return new Vector3(basePos.x + x, y, basePos.z + z);
    }

    public void NotifyMineralCollected(GameObject mineral)
    {
        if (activeMinerals.Contains(mineral))
        {
            activeMinerals.Remove(mineral);
        }

        PoolKey key = GetMineralKey(mineral);
        ObjectPoolManager.Instance.ReturnToPool(key, mineral);
    }

    private PoolKey GetMineralKey(GameObject obj)
    {
        if (obj.name.Contains("Mineral_1")) return mineral1Key;
        if (obj.name.Contains("Mineral_2")) return mineral2Key;
        return mineral3Key;
    }
}
