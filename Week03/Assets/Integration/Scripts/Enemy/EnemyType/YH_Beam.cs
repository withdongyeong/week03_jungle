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
        
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMoveManager = PlayerMoveManager.Instance;
        transform.position = playerMoveManager.transform.position + new Vector3(30f,0,30f);
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
        if ((playerMoveManager.transform.position - transform.position).magnitude < 30f)
        {
            
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(pos.x,transform.position.y,pos.z), Time.fixedDeltaTime * 10));
        }
        else
        {
            
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(pos.x,transform.position.y,pos.z), Time.fixedDeltaTime * 2));
        }
    }
}
