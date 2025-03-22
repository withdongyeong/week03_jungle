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
        //Ǯ���� �� ������Ʈ �̸�
        public String objectName;

        //������Ʈ Ǯ���� ������ ������Ʈ
        public GameObject prefab;

        //�̸� ������ ����
        public int maxCount;
    }

    //Ǯ �Ŵ����� �̱������� ����
    public static PoolManager instance;

    //�ν����Ϳ��� ������Ʈ Ǯ�� �����ֵ��� ����. ���� ������Ʈ Ǯ�� ���ο� ������ �߰��Ϸ��� �ν����Ϳ��� ������Ʈ ���� �ۼ��Ͻø� �˴ϴ�.
    [SerializeField] private ObjectInfo[] objectInofs;

    //������Ʈ Ǯ'��'�� �����ϱ� ���� ��ųʸ�
    public Dictionary<string, IObjectPool<GameObject>> pooldic { get; private set; } = new();

    //������Ʈ Ǯ�� ä��� ���� �� Ǯ�� ������ ������ ���� ��ųʸ�
    public Dictionary<string, GameObject> poolGoDic { get; private set; } = new();

    //��ųʸ��� key������ �־��� �̸�
    private string objectName;

    //������Ʈ Ǯ�� �غ� �Ȱ��� Ȯ��
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
                Debug.Log("�̹� ��ϵ� ������Ʈ�Դϴ�" + objectInofs[idx].objectName);
                return;
            }
            poolGoDic.Add(objectInofs[idx].objectName, objectInofs[idx].prefab);
            pooldic.Add(objectInofs[idx].objectName, pool);

            //�̸� ������Ʈ ä���
            for (int i=0; i < objectInofs[idx].maxCount; i++)
            {
                objectName = objectInofs[idx].objectName;
                GameObject poolableGo = CreatePoolItem();
                Debug.Log("���������� ���� �ǽ�");
                poolableGo.GetComponent<IPoolable>().pool.Release(poolableGo);
            }
        }
    }


    //Ǯ�� ������ �ֱ�
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

    //������Ʈ�� ��ġ/�����̼��� �ʱ�ȭ�� �ʿ��ϸ�, �Ű������� �־��ֽø� �˴ϴ�.
    public GameObject GetGo(string goName,Vector3 position, Quaternion rotation)
    {
        objectName = goName;

        if(poolGoDic.ContainsKey(goName) == false)
        {
            Debug.Log("����! ������Ʈ Ǯ�� ���� ���� ������Ʈ �̸��Դϴ�" + goName);
            return null;
        }
        var pooledObject = pooldic[goName].Get();
        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;
        return pooledObject;
    }
}
