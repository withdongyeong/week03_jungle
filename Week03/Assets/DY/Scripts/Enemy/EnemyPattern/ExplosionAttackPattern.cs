using UnityEngine;
using System.Collections;

public class ExplosionAttackPattern : IEnemyAttackPattern
{
    private PoolKey warningKey;
    private PoolKey explosionKey;
    private Transform attacker;
    private ExplosionAttackData data; // Null -> Use defalut global settings

    private float nextAttackTime;

    public ExplosionAttackPattern(PoolKey warningKey, PoolKey explosionKey, Transform attacker, ExplosionAttackData data = null)
    {
        this.warningKey = warningKey;
        this.explosionKey = explosionKey;
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
        Vector3 targetPos = GetRandomExplosionPosition();
        GameObject warning = ObjectPoolManager.Instance.SpawnFromPool(warningKey, targetPos, Quaternion.identity);

        float warningTime = data ? data.warningTime : GlobalSettings.Instance.defaultExplosionWarningTime;
        Vector3 scale = data ? data.warningScale : GlobalSettings.Instance.defaultExplosionWarningScale;
        warning.transform.localScale = scale;

        attacker.GetComponent<MonoBehaviour>().StartCoroutine(DelayExplosion(targetPos, warning));
        SetNextAttackTime();
    }


    private void SetNextAttackTime()
    {
        float min = data ? data.minInterval : GlobalSettings.Instance.defaultExplosionAttackMinInterval;
        float max = data ? data.maxInterval : GlobalSettings.Instance.defaultExplosionAttackMaxInterval;
        nextAttackTime = Time.time + Random.Range(min, max);
    }

    private IEnumerator DelayExplosion(Vector3 pos, GameObject warning)
    {
        float warningTime = data ? data.warningTime : GlobalSettings.Instance.defaultExplosionWarningTime;
        float effectTime = data ? data.effectTime : GlobalSettings.Instance.defaultExplosionEffectTime;

        yield return new WaitForSeconds(warningTime);
        if (warning != null) ObjectPoolManager.Instance.ReturnToPool(PoolKey.Warning, warning);

        GameObject explosion = ObjectPoolManager.Instance.SpawnFromPool(explosionKey, pos, Quaternion.identity);
        yield return new WaitForSeconds(effectTime);
        if (explosion != null) ObjectPoolManager.Instance.ReturnToPool(PoolKey.ExplosionEffect, explosion);
    }

    private Vector3 GetRandomExplosionPosition()
    {
        var player = HW_PlayerStateController.Instance;
        if (player == null) return Vector3.zero;

        float range = data ? data.range : GlobalSettings.Instance.defaultExplosionRange;
        float height = data ? data.height : GlobalSettings.Instance.defaultExplosionHeight;

        Vector3 basePos = player.transform.position;
        float x = Random.Range(-range, range);
        float z = Random.Range(-range, range);

        return new Vector3(basePos.x + x, height, basePos.z + z);
    }
}
