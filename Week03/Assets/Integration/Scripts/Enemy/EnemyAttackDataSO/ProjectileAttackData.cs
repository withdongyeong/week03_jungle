using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileAttackData", menuName = "Enemy/ProjectileAttackData")]
public class ProjectileAttackData : ScriptableObject
{
    [Header("Interval")]
    public float minInterval = 3f;
    public float maxInterval = 5f;

    [Header("Warning")]
    public float warningTime = 1f;
    public Vector3 warningScale = new Vector3(1f, 1f, 1f);

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
}
