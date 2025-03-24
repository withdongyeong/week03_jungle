using UnityEngine;
using System.Collections.Generic;


public class EnemyManager : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject explosionEnemyPrefab;
    public GameObject projectileEnemyPrefab;
    public GameObject cubeEnemyPrefab;
    public GameObject beamEnemyPrefab;
    public GameObject bossPrefab;
    private List<GameObject> normalEnemyPrefabs = new List<GameObject>();
    private GameObject beamEnemyInstance = null;
    private GameObject bossEnemyInstance = null;



    [Header("Explosion Prefabs (풀 등록용)")]
    public GameObject warningPrefab;
    public GameObject explosionPrefab;
    public GameObject laserPrefab;
    public GameObject laserExplosionPrefab;
    public GameObject dronePrefab;
    public GameObject pulsePrefab;
    public GameObject yamatoPrefab;
    public GameObject smallMissilePrefab;


    public PoolKey warningKey = PoolKey.Warning;
    public PoolKey explosionKey = PoolKey.ExplosionEffect;
    public PoolKey laserKey = PoolKey.ProjectileLaser;
    public PoolKey laserExplosionKey = PoolKey.LaserExplosion;
    public PoolKey drone = PoolKey.Drone;
    public PoolKey pulse = PoolKey.PulseProjectile;
    public PoolKey yamato = PoolKey.YamatoCannon;
    public PoolKey smallMissile = PoolKey.SmallMissile;

    private int currentEnemyCount = 0;
    private float spawnTimer = 0f;
    private float cubeSpawnTimer = 0f;

    private int MaxCount => GlobalSettings.Instance.maxEnemyCount;
    private float SpawnInterval => GlobalSettings.Instance.defaultSpawnInterval;
    private float SpawnRange => GlobalSettings.Instance.defaultSpawnRange;
    private bool warningTriggered = false;
    private bool bossTriggered = false;


    private void Awake()
    {
        if (explosionEnemyPrefab != null) normalEnemyPrefabs.Add(explosionEnemyPrefab);
        if (projectileEnemyPrefab != null) normalEnemyPrefabs.Add(projectileEnemyPrefab);
    }




    private void Start()
    {
        ObjectPoolManager.Instance.CreatePool(warningKey, warningPrefab, 20);
        ObjectPoolManager.Instance.CreatePool(explosionKey, explosionPrefab, 20);
        ObjectPoolManager.Instance.CreatePool(laserKey, laserPrefab, 20);
        ObjectPoolManager.Instance.CreatePool(laserExplosionKey, laserExplosionPrefab, 20);
        ObjectPoolManager.Instance.CreatePool(drone, dronePrefab, 10);
        ObjectPoolManager.Instance.CreatePool(pulse, pulsePrefab, 10);
        ObjectPoolManager.Instance.CreatePool(yamato, yamatoPrefab, 3);
        ObjectPoolManager.Instance.CreatePool(smallMissile, smallMissilePrefab, 10);


    }

    private void Update()
    {
        int mineral = GameInfoManager.Instance.Mineral;
        int stage = GameInfoManager.Instance.CurrentStage;


        if (!warningTriggered && mineral >= 4 && stage == 3)
        {
            warningTriggered = true;
            ShowWarning();
        }

        if (!bossTriggered && mineral >= 20 && stage == 3)
        {
            bossTriggered = true;
            SpawnBoss();
        }

        cubeSpawnTimer += Time.deltaTime;
        if (cubeSpawnTimer >= GlobalSettings.Instance.CubeSpawnInterval)
        {
            SpawnCube();
            cubeSpawnTimer = 0f;
        }
        if (currentEnemyCount >= MaxCount) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= SpawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }

        
    }

    private void ShowWarning()
    {
        LogManager.Instance.InvokeLine("bossWarning");
    }


    private void SpawnEnemy()
    {
        int stage = GameInfoManager.Instance.CurrentStage;

        if (beamEnemyInstance == null && beamEnemyPrefab != null && stage>=2)
        {
            Vector3 pos = GetRandomSpawnPosition();
            pos.y = 80f;
            beamEnemyInstance = Instantiate(beamEnemyPrefab, pos, Quaternion.identity);
            return;
        }
            
        // 일반 몬스터
        if (normalEnemyPrefabs.Count == 0) return;

        GameObject selected = normalEnemyPrefabs[Random.Range(0, normalEnemyPrefabs.Count)];
        if (selected == null) return;

        Vector3 spawnPos = GetRandomSpawnPosition();
        Instantiate(selected, spawnPos, Quaternion.identity);

        if (selected != cubeEnemyPrefab)
        {
            currentEnemyCount++;
        }
        
    }

    private void SpawnCube()
    {
        int stage = GameInfoManager.Instance.CurrentStage;
        if (stage < 2) return;

        Vector3 spawnPos = GetRandomSpawnPosition();
        Instantiate(cubeEnemyPrefab, spawnPos, Quaternion.identity);
    }

    public void SpawnBoss()
    {
        LogManager.Instance.InvokeLine("boss");
        int stage = GameInfoManager.Instance.CurrentStage;

        if (stage < 3) return;
        if (bossEnemyInstance != null) return;
        if (bossPrefab == null) return;

        Vector3 pos = new Vector3(0, 50, 240);
        bossEnemyInstance = Instantiate(bossPrefab, pos, Quaternion.identity);
        currentEnemyCount++;
    }



    private Vector3 GetRandomSpawnPosition()
    {
        var player = HW_PlayerStateController.Instance;
        if (player == null) return Vector3.zero;

        Vector3 basePos = player.transform.position;
        Vector3 forward = player.transform.forward;
        float range = SpawnRange;

        float forwardOffset = GlobalSettings.Instance.spawnForwardOffset; // 고정된 앞쪽 거리

        // 중심 위치: 플레이어 앞쪽
        Vector3 center = basePos + forward * forwardOffset;

        // 중심 기준 원형 범위 내 랜덤 위치
        Vector2 circleOffset = Random.insideUnitCircle * range;
        float y = basePos.y;

        return new Vector3(center.x + circleOffset.x, y, center.z + circleOffset.y);
    }


}
