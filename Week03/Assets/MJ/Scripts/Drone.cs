using System.Collections;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Drone : MonoBehaviour, IPoolable
{
    public IObjectPool<GameObject> pool { get; set; }
    float time = 0f;
    int targetnum;
    Vector3 targetDir;
    Rigidbody rb;
    [SerializeField] private float speed;
    Vector3 startPosition;
    LineRenderer lineRenderer;
    float timeForFire = 0;
    bool isDroneFire;
    Vector3 finalPosition;
    RaycastHit hit;
    bool isTracking;

    public void ReleaseObject()
    {
        pool.Release(gameObject);
    }

    public void Init(int i)
    {
        targetnum = i;
        FindTargetPosition();
    }

    private void OnEnable()
    {
        StartCoroutine(DroneRoutine());
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDroneFire)
        {
            if (timeForFire < 1f)
            {
                lineRenderer.endWidth = 0.1f;
                lineRenderer.startWidth = 0.1f;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, HW_PlayerStateController.Instance.transform.position);
                transform.rotation = Quaternion.LookRotation(HW_PlayerStateController.Instance.transform.position - transform.position);
                finalPosition = HW_PlayerStateController.Instance.transform.position;

            }
            else if(1f<timeForFire && timeForFire < 1.1f)
            {
                isTracking = false;
            }
            else if (1.2f < timeForFire)
            {
                isDroneFire = false;
                lineRenderer.endWidth = 2f;
                lineRenderer.startWidth = 2f;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 800, 1 << 6))
                {
                    GameInfoManager.Instance.UpdateHP(-5);
                }
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position + transform.forward * 800);
                Invoke("TurnOffLaser", 0.05f);

            }
            timeForFire += Time.deltaTime;
        }
      

        
    }

    IEnumerator DroneRoutine()
    {
        time = 0f;
        yield return new WaitForSeconds(1f);
        isDroneFire = true;
        lineRenderer.positionCount = 2;
        while (time<2f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        startPosition = transform.position;
        while (time<5.5f)
        {
            transform.rotation = Quaternion.LookRotation(HW_PlayerStateController.Instance.transform.position - transform.position);
            transform.position = Vector3.Slerp(startPosition, HW_PlayerStateController.Instance.transform.position + targetDir, (time - 2) / 3f);
            time += Time.deltaTime;
            yield return null;
        }
        isDroneFire = true;
        isTracking = true;
        lineRenderer.positionCount = 2;
        while(time<9f)
        {
            if(isTracking)
                transform.position = Vector3.Slerp(startPosition, HW_PlayerStateController.Instance.transform.position + targetDir, (time - 2) / 3f);
            time += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        ReleaseObject();
    }

    void FindTargetPosition()
    {
        if(targetnum == 0)
        {
            targetDir = new Vector3(0, 6, 10);
        }
        else if(targetnum == 1)
        {
            targetDir = new Vector3(0, 6, -10);
        }
        else if(targetnum == 2)
        {
            targetDir = new Vector3(5 * Mathf.Sqrt(3), 6, 5);
        }
        else if(targetnum == 3)
        {
            targetDir = new Vector3(-5 * Mathf.Sqrt(3), 6, 5);
        }
        else if (targetnum == 4)
        {
            targetDir = new Vector3(5, 6, 5 * Mathf.Sqrt(3));
        }
        else if (targetnum == 5)
        {
            targetDir = new Vector3(5, 6, -5 * Mathf.Sqrt(3));
        }
    }

    void TurnOffLaser()
    {
        lineRenderer.positionCount = 0;
        timeForFire = 0;
    }
  
}
