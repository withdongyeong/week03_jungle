using UnityEngine;
using System.Collections;

public class ProjectileAttackPattern : IEnemyAttackPattern
{
    private PoolKey warningKey;
    private PoolKey projectileKey;
    private Transform attacker;
    private ProjectileAttackData data;

    private float nextAttackTime;

    public ProjectileAttackPattern(PoolKey warningKey, PoolKey projectileKey, Transform attacker, ProjectileAttackData data = null)
    {
        this.warningKey = warningKey;
        this.projectileKey = projectileKey;
        this.attacker = attacker;
        this.data = data;

        SetNextAttackTime();
    }

    public bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    public void ExecuteAttack()
    {
        Vector3 targetPos = GetRandomTargetPosition();
        GameObject warning = ObjectPoolManager.Instance.SpawnFromPool(warningKey, targetPos, Quaternion.identity);

        Vector3 scale = data ? data.warningScale : GlobalSettings.Instance.defaultProjectileWarningScale;
        warning.transform.localScale = scale;
        Vector3 localPos = warning.transform.localPosition;
        localPos.y = GlobalSettings.Instance.defaultProjectileWarningPositionY; ;
        warning.transform.localPosition = localPos;

        attacker.GetComponent<MonoBehaviour>().StartCoroutine(DelayedShoot(targetPos, warning));
        SetNextAttackTime();
    }

    private IEnumerator DelayedShoot(Vector3 targetPos, GameObject warning)
    {
        float warningTime = data ? data.warningTime : GlobalSettings.Instance.defaultProjectileWarningTime;
        yield return new WaitForSeconds(warningTime);

        if (warning != null) ObjectPoolManager.Instance.ReturnToPool(PoolKey.Warning, warning);

        Vector3 spawnPos = GetProjectileSpawnPosition();
        GameObject projectile = ObjectPoolManager.Instance.SpawnFromPool(projectileKey, spawnPos, Quaternion.identity);

        Vector3 dir = (targetPos - spawnPos).normalized;
        projectile.transform.rotation = Quaternion.LookRotation(dir);

        float speed = data ? data.projectileSpeed : GlobalSettings.Instance.defaultProjectileSpeed;
        attacker.GetComponent<MonoBehaviour>().StartCoroutine(MoveProjectile(projectile, targetPos, speed));
    }

    private IEnumerator MoveProjectile(GameObject projectile, Vector3 targetPos, float speed)
    {
        if (projectile == null) yield break;

        Vector3 startPos = projectile.transform.position;
        float totalDistance = Vector3.Distance(startPos, targetPos);
        float traveled = 0f;

        while (projectile != null && traveled < totalDistance)
        {
            Vector3 dir = (targetPos - projectile.transform.position).normalized;
            float step = speed * Time.deltaTime;
            projectile.transform.position += dir * step;
            traveled += step;
            yield return null;
        }

        if (projectile != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(PoolKey.ProjectileLaser, projectile);

            GameObject laserExplosion = ObjectPoolManager.Instance.SpawnFromPool(PoolKey.LaserExplosion, targetPos, Quaternion.identity);

            Vector3 targetScale = GlobalSettings.Instance.laserExplosionScale;
            Vector3 startScale = targetScale * 0.5f;
            laserExplosion.transform.localScale = startScale;

            float scaleUpTime = GlobalSettings.Instance.explosionScaleUpTime;

            attacker.GetComponent<MonoBehaviour>().StartCoroutine(
                ScaleUpEffect(laserExplosion.transform, startScale, targetScale, scaleUpTime)
            );

            attacker.GetComponent<MonoBehaviour>().StartCoroutine(
                DisableAfterSeconds(PoolKey.LaserExplosion, laserExplosion, scaleUpTime)
            );
        }

    }

    private IEnumerator DisableAfterSeconds(PoolKey key, GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (obj != null)
            ObjectPoolManager.Instance.ReturnToPool(key, obj);
    }

    private IEnumerator ScaleUpEffect(Transform target, Vector3 startScale, Vector3 endScale, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            target.localScale = Vector3.Lerp(startScale, endScale, t);
            timer += Time.deltaTime;
            yield return null;
        }
        target.localScale = endScale;
    }


    private Vector3 GetRandomTargetPosition()
    {
        var player = HW_PlayerStateController.Instance;
        if (player == null) return Vector3.zero;

        Vector3 basePos = player.transform.position;
        float range = GlobalSettings.Instance.attackRandomRange;
        float height = GlobalSettings.Instance.defaultExplosionHeight;

        float x = Random.Range(-range, range);
        float z = Random.Range(-range, range);

        return new Vector3(basePos.x + x, height, basePos.z + z);
    }

    private Vector3 GetProjectileSpawnPosition()
    {
        var player = HW_PlayerStateController.Instance;
        if (player == null) return Vector3.zero;

        Vector3 basePos = player.transform.position;

        float height = GlobalSettings.Instance.defaultProjectileSpawnHeight;
        float range = GlobalSettings.Instance.defaultProjectileSpawnRange;

        float offsetX = Random.Range(-range, range);
        float offsetZ = Random.Range(-range, range);
        float offsetY = Random.Range(-height * 0.2f, height * 0.2f);

        return new Vector3(
            basePos.x + offsetX,
            basePos.y + height + offsetY,
            basePos.z + offsetZ
        );
    }


    private void SetNextAttackTime()
    {
        float min = data ? data.minInterval : GlobalSettings.Instance.defaultProjectileAttackMinInterval;
        float max = data ? data.maxInterval : GlobalSettings.Instance.defaultProjectileAttackMaxInterval;
        nextAttackTime = Time.time + Random.Range(min, max);
    }

}
