using System;
using UnityEngine;

public class YH_Garen : MonoBehaviour
{
    [SerializeField]
    private float speed = 100f;

    private Transform stick;
    private bool isAttacking = false;
    private float time; 
    private float maxTime; //사전 준비 시간
    private Material mesh;
    private void Awake()
    {
        stick = transform.GetChild(0);
        mesh = stick.GetComponent<MeshRenderer>().material;
    }

    private void OnEnable()
    {
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        time = 0;
        maxTime = 2f;
        stick.localPosition = new Vector3(0, 0, 0);
        stick.localScale = new Vector3(0, 1, 1);
        isAttacking = false;
        mesh.color = Color.yellow;
    }
    
    private void Update()
    {

        if (isAttacking)
        {
            time += Time.deltaTime;
            transform.Rotate(Vector3.up, GlobalSettings.Instance.spinSpeed * Time.deltaTime);
            if (time >= 5)
            {
                Destroy(gameObject);
            }
        }
        else
            StartAction();
    }

    private void StartAction()
    {
        time += Time.deltaTime;
        stick.localScale = Vector3.Lerp(new Vector3(0,1,1), new Vector3(30,1,1), time/maxTime);
        stick.localPosition = Vector3.Lerp(new Vector3(0,0,0),new Vector3(-17f,0,0), time/maxTime);
        if (time >= maxTime)
        {
            isAttacking = true;
            stick.localPosition = new Vector3(-17,0,0);
            stick.localScale = new Vector3(30,1,1);
            time = 0;
            mesh.color = Color.red;
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;
        if (other.CompareTag("Player"))
        {
            GameInfoManager.Instance.UpdateHP(GlobalSettings.Instance.spinDamage);
        }
    }
}
