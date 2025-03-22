using UnityEngine;
using UnityEngine.Pool;

public class DroneBullet : MonoBehaviour,IPoolable
{
    public IObjectPool<GameObject> pool { get; set; }

    public void ReleaseObject()
    {
        pool.Release(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * 10);
    }
}
