using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    public PoolKey warningKey = PoolKey.Warning;
    public PoolKey projectileKey = PoolKey.ProjectileLaser;
    public ProjectileAttackData attackData; // Optional

    private IEnemyAttackPattern attackPattern;

    private void Start()
    {
        attackPattern = new ProjectileAttackPattern(warningKey, projectileKey, transform, attackData);
    }

    private void Update()
    {
        if (attackPattern.CanAttack())
        {
            attackPattern.ExecuteAttack();
        }
    }
}
