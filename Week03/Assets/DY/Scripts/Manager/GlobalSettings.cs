using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance;

    [Header("Object Pool")]
    public float objectPoolCleanupInterval = 30f;

    [Header("Default Explosion Settings")]
    public float defaultExplosionAttackMinInterval = 5f;
    public float defaultExplosionAttackMaxInterval = 8f;
    public Vector3 defaultExplosionWarningScale = new Vector3(10f, 10f, 10f);
    public float defaultExplosionWarningTime = 1.5f;
    public float defaultExplosionEffectTime = 2f;
    public float defaultExplosionRange = 3f;
    public float defaultExplosionHeight = 1f;

    [Header("Explosion Enemy Spawn Settings")]
    public int maxExplosionEnemyCount = 5;
    public float explosionSpawnInterval = 5f;
    public float explosionSpawnRange = 10f;


    [Header("Default Projectile Settings")]
    public float defaultProjectileSpeed = 100f;
    public float defaultProjectileWarningTime = 0.5f;

    public Vector3 defaultProjectileWarningScale = new Vector3(3f, 3f, 3f);
    public float defaultProjectileAttackMinInterval = 3f;
    public float defaultProjectileAttackMaxInterval = 5f;

    public float defaultProjectileSpawnHeight = 25f;
    public float defaultProjectileSpawnRange = 15f;
    public float laserExplosionEffectTime = 1f;
    public Vector3 laserExplosionScale = new Vector3(3f, 3f, 3f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
