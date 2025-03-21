using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Drone : MonoBehaviour, IPoolable
{
    public IObjectPool<GameObject> pool { get; set; }
    float time = 0f;
   

    public void ReleaseObject()
    {
        pool.Release(gameObject);
    }

    private void OnEnable()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }

    IEnumerator DroneRoutine()
    {
        time = 0f;
        while(time<2f)
        {
            transform.rotation = Quaternion.LookRotation(HW_PlayerStateController.Instance.transform.position - transform.position);
            time += Time.deltaTime;
            yield return null;
        }
        DroneFire();
        while(time<6f)
        {
            Vector3 targetDir = HW_PlayerStateController.Instance.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.Slerp(transform.forward, targetDir, 0.01f));
            yield return null;
            yield return null;
        }

    }

    void DroneFire()
    {
        
    }
}
