using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class YamatoCannon : MonoBehaviour, IPoolable
{
    public IObjectPool<GameObject> pool { get; set; }

    Rigidbody rb;


    private int layerMask = (1 << 6) | (1 << 7);

    float time = 0f;

    bool isAimed = false;

    bool isGround = false;

    [SerializeField] private float speed;

    [SerializeField] private GameObject warning;

    GameObject currentWarning;

    RaycastHit hit;
    public void ReleaseObject()
    {
        StartCoroutine(DestroyThenReturn());
    }

    IEnumerator DestroyThenReturn()
    {
        if (currentWarning != null)
        {
            Destroy(currentWarning);
            yield return null; // 한 프레임 기다려서 확실히 파괴
        }

        ObjectPoolManager.Instance.ReturnToPool(PoolKey.YamatoCannon, gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        time = 0f;
        isAimed = false;
        isGround = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(2.5f >=time)
        {
            transform.localScale = new Vector3(1, 1, 1) * (1 + time * 3);
            time += Time.deltaTime;
        }
        else if(time>2.5f && !isGround)
        {
            if(!isAimed)
            {
                transform.rotation = Quaternion.LookRotation(HW_PlayerStateController.Instance.transform.position - transform.position);
                isAimed = true;
                if (Physics.Raycast(transform.position, transform.forward, out hit,1000,1<<7))
                {
                    Debug.Log("레이케스트 완료");
                    currentWarning = Instantiate(warning, hit.point, transform.rotation);
                }
            }

            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4f, layerMask);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].CompareTag("Player"))
                {
                    GameInfoManager.Instance.UpdateHP(-10);
                    ReleaseObject();
                }
            }
            Collider[] groundColliders = Physics.OverlapSphere(transform.position, 2f, 1<<7);
            if (groundColliders.Length > 0)
                isGround = true;

        }
        else if(isGround)
        {
            time += Time.deltaTime;
            transform.localScale = new Vector3(1, 1, 1) * (-268.375f + time * 110.75f);
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, (-268.375f + time * 110.75f)/2, 1<<6);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].CompareTag("Player"))
                {
                    GameInfoManager.Instance.UpdateHP(-1);
                }
            }
            if (time > 4.5f)
                ReleaseObject();
        }
    }
  
}
