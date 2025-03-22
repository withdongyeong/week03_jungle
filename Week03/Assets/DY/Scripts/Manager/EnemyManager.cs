using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject explosionEnemyPrefab;
    public GameObject projectileEnemyPrefab;

    [Header("Explosion Prefabs (풀 등록용)")]
    public GameObject warningPrefab;
    public GameObject explosionPrefab;
    public GameObject laserPrefab;
    public GameObject laserExplosionPrefab;

    public PoolKey warningKey = PoolKey.Warning;
    public PoolKey explosionKey = PoolKey.ExplosionEffect;
    public PoolKey laserKey = PoolKey.ProjectileLaser;
    public PoolKey laserExplosionKey = PoolKey.LaserExplosion;

    private int currentEnemyCount = 0;
    private float spawnTimer = 0f;

    private int MaxCount => GlobalSettings.Instance.maxEnemyCount;
    private float SpawnInterval => GlobalSettings.Instance.defaultSpawnInterval;
    private float SpawnRange => GlobalSettings.Instance.defaultSpawnRange;

    private void Start()
    {
        ObjectPoolManager.Instance.CreatePool(warningKey, warningPrefab, 10);
        ObjectPoolManager.Instance.CreatePool(explosionKey, explosionPrefab, 10);
        ObjectPoolManager.Instance.CreatePool(laserKey, laserPrefab, 10);
        ObjectPoolManager.Instance.CreatePool(laserExplosionKey, laserExplosionPrefab, 10);

    }

    private void Update()
    {
        if (currentEnemyCount >= MaxCount) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= SpawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        GameObject selected = Random.value < 0.5f ? explosionEnemyPrefab : projectileEnemyPrefab;
        if (selected == null) return;

        Vector3 spawnPos = GetRandomSpawnPosition();
        Instantiate(selected, spawnPos, Quaternion.identity);
        currentEnemyCount++;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var player = HW_PlayerStateController.Instance;
        if (player == null) return Vector3.zero;

        Vector3 basePos = player.transform.position;
        float range = SpawnRange;

        float x = Random.Range(-range, range);
        float z = Random.Range(-range, range);
        float y = basePos.y;

        return new Vector3(basePos.x + x, y, basePos.z + z);
    }
}
