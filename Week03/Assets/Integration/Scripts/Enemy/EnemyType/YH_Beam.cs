using System;
using System.Collections;
using UnityEngine;

public class YH_Beam : MonoBehaviour
{
    [SerializeField]
    private PlayerMoveManager playerMoveManager;
    private Rigidbody rb;
    [SerializeField]
    private Vector3 pos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        
        playerMoveManager = PlayerMoveManager.Instance;
        StartCoroutine(MoveBeamCoroutine());
    }

    private void Update()
    {
        MoveBeam();
    }

    IEnumerator MoveBeamCoroutine()
    {
        while (true)
        {
            pos = playerMoveManager.transform.position;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void MoveBeam()
    {
        if ((playerMoveManager.transform.position - transform.position).magnitude < 10f)
        {
            
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(pos.x,transform.position.y,pos.z), Time.fixedDeltaTime * GlobalSettings.Instance.beamSpeed * 10f));
        }
        else
        {
            
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(pos.x,transform.position.y,pos.z), Time.fixedDeltaTime * GlobalSettings.Instance.beamSpeed));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            GameInfoManager.Instance.UpdateHP(GlobalSettings.Instance.beamEnterDamage);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            GameInfoManager.Instance.UpdateHP(GlobalSettings.Instance.beamStayDamage);
        }
    }
}
