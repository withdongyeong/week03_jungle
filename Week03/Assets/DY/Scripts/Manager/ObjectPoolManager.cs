using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
    private Dictionary<PoolKey, Queue<GameObject>> poolDict = new();
    private Dictionary<PoolKey, GameObject> prefabDict = new();
    private float cleanupInterval;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StartCoroutine(AutoCleanupRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cleanupInterval = GlobalSettings.Instance.objectPoolCleanupInterval;
    }

    public void CreatePool(PoolKey key, GameObject prefab, int count)
    {
        if (poolDict.ContainsKey(key)) return;

        Queue<GameObject> objectQueue = new();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            objectQueue.Enqueue(obj);
        }

        poolDict.Add(key, objectQueue);
        prefabDict.Add(key, prefab);
    }

    public GameObject SpawnFromPool(PoolKey key, Vector3 position, Quaternion rotation)
    {
        if (!poolDict.ContainsKey(key)) return null;

        Queue<GameObject> pool = poolDict[key];
        GameObject obj = null;

        while (pool.Count > 0)
        {
            obj = pool.Dequeue();
            if (obj != null && !obj.activeInHierarchy)
                break;
        }

        if (obj == null)
        {
            GameObject prefab = prefabDict[key];
            obj = Instantiate(prefab);
            Debug.LogWarning($"[Pool:{key}] 풀 부족 → 새 인스턴스 생성");
        }

        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);

        return obj;
    }

    public void ReturnToPool(PoolKey key, GameObject obj)
    {
        if (!poolDict.ContainsKey(key)) return;

        obj.SetActive(false);
        poolDict[key].Enqueue(obj);
    }



    private IEnumerator AutoCleanupRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(cleanupInterval);
            CleanupInactiveObjects();
        }
    }

    private void CleanupInactiveObjects()
    {
        foreach (var key in poolDict.Keys.ToList())
        {
            Queue<GameObject> pool = poolDict[key];
            Queue<GameObject> newQueue = new();

            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                if (obj != null) newQueue.Enqueue(obj);
            }

            poolDict[key] = newQueue;
        }

        Debug.Log("[ObjectPoolManager] 풀 정리 완료");
    }

}
