using UnityEngine;

public class MineralEffectReturner : MonoBehaviour
{
    public PoolKey poolKey;
    public float lifetime = 1.5f;

    private float timer;

    private void OnEnable()
    {
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            ObjectPoolManager.Instance.ReturnToPool(poolKey, gameObject);
        }
    }
}
