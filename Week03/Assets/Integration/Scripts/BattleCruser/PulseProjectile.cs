using UnityEngine;
using UnityEngine.Pool;

public class PulseProjectile : MonoBehaviour , IPoolable
{
    Rigidbody rb;
    public IObjectPool<GameObject> pool { get; set; }

    private int layerMask = (1 << 6) | (1 << 7);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f,layerMask);
        for(int i =0; i<hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                GameInfoManager.Instance.UpdateHP(-5);
            }
        }
        if(hitColliders.Length > 0)
            ReleaseObject();
    }
    public void ReleaseObject()
    {
        ObjectPoolManager.Instance.ReturnToPool(PoolKey.PulseProjectile, gameObject);
    }


}
