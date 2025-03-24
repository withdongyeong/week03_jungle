using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance;

    [Header("Object Pool")]
    public float objectPoolCleanupInterval = 30f;

    [Header("Enemy Damages")]
    public int laserDamage = -2;
    public int explosionDamage = -10;

    [Header("Default Explosion Settings")]
    public float defaultExplosionAttackMinInterval = 5f;
    public float defaultExplosionAttackMaxInterval = 8f;
    public Vector3 defaultExplosionWarningScale = new Vector3(10f, 10f, 10f);
    public float defaultExplosionWarningTime = 1.5f;
    public float defaultExplosionEffectTime = 2f;
    public float attackRandomRange = 10f;
    public float attackForwardOffset = 10f;
    public float defaultExplosionHeight = 1f;
    public Vector3 defaultExplosionScale = new Vector3(10f, 10f, 10f);
    public float explosionScaleUpTime = 0.5f;

    [Header("Default Enemy Settings")]
    public int maxEnemyCount = 20;
    public float defaultSpawnInterval = 3f;
    public float defaultSpawnRange = 30f;
    public float spawnForwardOffset = 30f;

    [Header("Default Projectile Settings")]
    public float defaultProjectileSpeed = 100f;
    public float defaultProjectileWarningTime = 0.5f;

    public Vector3 defaultProjectileWarningScale = new Vector3(3f, 7f, 3f);
    public float defaultProjectileAttackMinInterval = 3f;
    public float defaultProjectileAttackMaxInterval = 5f;
    public float defaultProjectileWarningPositionY = 0.5f;
    public float defaultProjectileSpawnHeight = 25f;
    public float defaultProjectileSpawnRange = 15f;
    public float laserExplosionEffectTime = 1f;
    public Vector3 laserExplosionScale = new Vector3(3f, 3f, 3f);

    [Header("Default Mineral Settings")]
    public int maxMineralCount = 20;
    public float mineralSpawnInterval = 3f;
    public float mineralSpawnRange = 80f;

    public float mineral1Probability = 0.6f;
    public float mineral2Probability = 0.3f;
    public float mineral3Probability = 0.1f;
    [Header("Mineral Score Table")]
    public int mineral1Score = 4;
    public int mineral2Score = 8;
    public int mineral3Score = 16;
    
    [Header("Beam Settings")]
    public int beamEnterDamage = -5;
    public int beamStayDamage = -1;
    public float beamSpeed = 2;
    
    [Header("Spin Settings")]
    public float CubeSpawnInterval = 1f;
    public int spinDamage = -5;
    public float spinSpeed = 100;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
