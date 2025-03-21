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
    public void ReleaseObject()
    {
        pool.Release(gameObject);
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
            transform.localScale = new Vector3(1, 1, 1) * (-141.5f + time * 60);
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, (-141.5f + time * 60)/2, 1<<6);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].CompareTag("Player"))
                {
                    GameInfoManager.Instance.UpdateHP(-1);
                }
            }
            if (time > 7.5f)
                ReleaseObject();
        }
    }
}
