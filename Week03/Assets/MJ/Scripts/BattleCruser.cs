using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCruser : MonoBehaviour
{
    Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = HW_PlayerStateController.Instance.transform;
        DroneAttack();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FireSmallMissile()
    {
        Debug.Log("발사");
        for(int b=0; b<3; b++)
        {
            for (int i = 0; i < 6; i++)
            {
                PoolManager.instance.GetGo("SmallMissile", transform.position + new Vector3(-15.2f, 5 * b - 5, i * 4 - 10), Quaternion.LookRotation(transform.right));
                PoolManager.instance.GetGo("SmallMissile", transform.position + new Vector3(15.2f, 5 * b - 5, i * 4 - 10), Quaternion.LookRotation(-transform.right));
            }
        }

    }

    private void ComboAttack()
    {
        StartCoroutine(PulseGun());
    }
    IEnumerator PulseGun()
    {
        for(int i=0; i<3; i++)
        {
            PoolManager.instance.GetGo(("PulseProjectile"), transform.position + new Vector3(5 * i - 5, -15.1f, 0), transform.rotation);
        }
        yield return new WaitForSeconds(0.5f);
        for(int b=0; b<2; b++)
        {
            for (int i = 0; i < 3; i++)
            {
                PoolManager.instance.GetGo(("PulseProjectile"), transform.position + new Vector3(5 * i - 5, -15.1f, 10*b - 5), transform.rotation);
            }
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++)
        {
            PoolManager.instance.GetGo(("PulseProjectile"), transform.position + new Vector3(5 * i - 5, -15.1f, 0), transform.rotation);
        }
        yield return new WaitForSeconds(0.5f);
        FireYamato();
    }

    void FireYamato()
    {
        PoolManager.instance.GetGo("YamatoCannon", transform.position + new Vector3(0, 0, -24), transform.rotation);
    }

    void DroneAttack()
    {
        List<int> randomList = new() { 0, 1, 2, 3, 4, 5 };
        for(int i=0; i<3; i++)
        {
            var drone = PoolManager.instance.GetGo("Drone", transform.position + new Vector3(6 * i - 33, 0, 0), transform.rotation);
            int randomNum = randomList[Random.Range(0, randomList.Count)];
            drone.GetComponent<Drone>().Init(randomNum);
            randomList.Remove(randomNum);
        }
        for (int i = 0; i < 3; i++)
        {
            var drone = PoolManager.instance.GetGo("Drone", transform.position + new Vector3(-6 * i + 33, 0, 0), transform.rotation);
            int randomNum = randomList[Random.Range(0, randomList.Count)];
            drone.GetComponent<Drone>().Init(randomNum);
            randomList.Remove(randomNum);
        }

    }

    void Attack()
    {
        int attackNum = Random.Range(0, 2);
        if(attackNum == 0)
        {
            StartCoroutine(PulseGun());
        }
        else
        {
            FireSmallMissile();
        }
    }
}
