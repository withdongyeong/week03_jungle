using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [Serializable]
    private class ObjectInfo
    {
        //풀링에 쓸 오브젝트 이름
        public String objectName;

        //오브젝트 풀에서 관리할 오브젝트
        public GameObject prefab;

        //미리 생성할 개수
        public int maxCount;
    }

    //풀 매니저를 싱글톤으로 선언
    public static PoolManager instance;

    //인스펙터에서 오브젝트 풀에 넣을애들을 선정. 만약 오브젝트 풀에 새로운 프리팹 추가하려면 인스펙터에서 오브젝트 인포 작성하시면 됩니다.
    [SerializeField] private ObjectInfo[] objectInofs;

    //오브젝트 풀'들'을 관리하기 위한 딕셔너리
    public Dictionary<string, IObjectPool<GameObject>> pooldic { get; private set; } = new();

    //오브젝트 풀을 채우기 위해 각 풀의 프리팹 정보를 담은 딕셔너리
    public Dictionary<string, GameObject> poolGoDic { get; private set; } = new();

    //딕셔너리에 key값으로 넣어줄 이름
    private string objectName;

    //오브젝트 풀링 준비 된건지 확인
    public bool isReady { get; private set; }

    private void Awake()
    {
        if (PoolManager.instance == null)
            instance = this;
        else
            Destroy(gameObject);
        Init();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Init()
    {
        isReady = false;
        for (int idx = 0; idx < objectInofs.Length; idx++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolGo, true,
                objectInofs[idx].maxCount, objectInofs[idx].maxCount);
            if (poolGoDic.ContainsKey(objectInofs[idx].objectName))
            {
                Debug.Log("이미 등록된 오브젝트입니다" + objectInofs[idx].objectName);
                return;
            }
            poolGoDic.Add(objectInofs[idx].objectName, objectInofs[idx].prefab);
            pooldic.Add(objectInofs[idx].objectName, pool);

            //미리 오브젝트 채우기
            for (int i=0; i < objectInofs[idx].maxCount; i++)
            {
                objectName = objectInofs[idx].objectName;
                GameObject poolableGo = CreatePoolItem();
                Debug.Log("오류났으면 여기 의심");
                poolableGo.GetComponent<IPoolable>().pool.Release(poolableGo);
            }
        }
    }


    //풀에 아이템 넣기
    private GameObject CreatePoolItem()
    {
        GameObject poolGo = Instantiate(poolGoDic[objectName]);
        poolGo.GetComponent<IPoolable>().pool = pooldic[objectName];
        return poolGo;
    }

    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    private void OnDestroyPoolGo(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    //오브젝트가 위치/로테이션의 초기화가 필요하면, 매개변수로 넣어주시면 됩니다.
    public GameObject GetGo(string goName,Vector3 position, Quaternion rotation)
    {
        objectName = goName;

        if(poolGoDic.ContainsKey(goName) == false)
        {
            Debug.Log("오류! 오브젝트 풀에 들어가지 않은 오브젝트 이름입니다" + goName);
            return null;
        }
        var pooledObject = pooldic[goName].Get();
        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;
        return pooledObject;
    }
}
