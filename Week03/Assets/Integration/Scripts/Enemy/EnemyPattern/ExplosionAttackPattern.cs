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

        if (warning != null)
            ObjectPoolManager.Instance.ReturnToPool(PoolKey.Warning, warning);

        GameObject explosion = ObjectPoolManager.Instance.SpawnFromPool(explosionKey, pos, Quaternion.identity);

        if (explosion != null)
        {
            Vector3 targetScale = GlobalSettings.Instance.defaultExplosionScale;
            Vector3 startScale = targetScale * 0.5f;
            explosion.transform.localScale = startScale;

            float scaleUpTime = GlobalSettings.Instance.explosionScaleUpTime;
            attacker.GetComponent<MonoBehaviour>().StartCoroutine(
                ScaleUpEffect(explosion.transform, startScale, targetScale, scaleUpTime)
            );
        }

        yield return new WaitForSeconds(effectTime);

        if (explosion != null)
            ObjectPoolManager.Instance.ReturnToPool(PoolKey.ExplosionEffect, explosion);
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

    private Vector3 GetRandomExplosionPosition()
    {
        var player = HW_PlayerStateController.Instance;
        if (player == null) return Vector3.zero;

        Vector3 basePos = player.transform.position;
        Vector3 forward = player.transform.forward;
        float range = GlobalSettings.Instance.attackRandomRange;
        float height = GlobalSettings.Instance.defaultExplosionHeight;

        float forwardOffset = GlobalSettings.Instance.attackForwardOffset; // 고정된 앞쪽 거리

        // 중심 위치: 플레이어 앞쪽
        Vector3 center = basePos + forward * forwardOffset;

        // 중심 기준 원형 범위 내 랜덤 위치
        Vector2 circleOffset = Random.insideUnitCircle * range;
        float y = height;

        return new Vector3(center.x + circleOffset.x, y, center.z + circleOffset.y);
    }
}
