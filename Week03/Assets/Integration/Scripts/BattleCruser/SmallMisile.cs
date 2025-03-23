using UnityEngine;
using UnityEngine.Pool;

public class SmallMisile : MonoBehaviour, IPoolable
{
    float time = 0f;
    Rigidbody rb;
    [SerializeField] private float speed;
    bool targeting = true;
    Vector3 startPosition;
    public IObjectPool<GameObject> pool { get; set; }

    private int layerMask = (1 << 6) | (1 << 7);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        time = 0f;
        targeting = true;
        rb.linearVelocity = Vector3.zero;
    }
    // Update is called once per frame
    void Update()
    {
        if(time == 0)
        {
            startPosition = transform.position;
        }
        time += Time.deltaTime;
        if (1.8f > time && time > 0)
        {
            transform.position = Vector3.Lerp(startPosition,new Vector3(transform.position.x, startPosition.y - 35 , transform.position.z),time/1.8f);
         

        }
        else if (3.4f > time && time > 1.8f)
        {
            transform.LookAt(HW_PlayerStateController.Instance.transform);

        }
        else if (10f > time && time > 3.4f)
        {
            rb.linearVelocity = transform.forward * speed * 3.5f;
            Vector3 targetDir = HW_PlayerStateController.Instance.transform.position - transform.position;
            if (Vector3.Angle(transform.forward, targetDir) <50f && targeting)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.Slerp(transform.forward, targetDir, 0.01f));
            }
            else if ((Vector3.Angle(transform.forward, targetDir) >= 50f))
            {
                targeting = false;
            }
        }
        else
            ReleaseObject();

        Vector3 point0 = transform.TransformPoint(0, 0, 0.5f);
        Vector3 point1 = transform.TransformPoint(0, 0, -0.5f);
        Collider[] hitColliders = Physics.OverlapCapsule(point0,point1,0.5f,layerMask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                GameInfoManager.Instance.UpdateHP(-10);
            }
        }
        if (hitColliders.Length > 0)
        {
            ReleaseObject();
        }
            
    }

    public void ReleaseObject()
    {
        ObjectPoolManager.Instance.ReturnToPool(PoolKey.SmallMissile, gameObject);
        Debug.Log("돌아감");
    }
}

