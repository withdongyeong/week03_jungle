using UnityEngine;

public class ExplosionEnemy : MonoBehaviour
{
    public PoolKey warningKey = PoolKey.Warning;
    public PoolKey explosionKey = PoolKey.ExplosionEffect;
    public ExplosionAttackData attackData; // Optional

    private IEnemyAttackPattern attackPattern;

    private void Start()
    {
        attackPattern = new ExplosionAttackPattern(warningKey, explosionKey, transform, attackData);
    }

    private void Update()
    {
        if (attackPattern.CanAttack())
        {
            attackPattern.ExecuteAttack();
        }
    }
}
