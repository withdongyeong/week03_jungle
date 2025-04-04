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
        InvokeRepeating("Attack", 1f, 8f);
    

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FireSmallMissile()
    {
        LogManager.Instance.InvokeLine("missile");
        Debug.Log("발사");
        for(int b=0; b<3; b++)
        {
            for (int i = 0; i < 6; i++)
            {
                Debug.Log("발사");
                ObjectPoolManager.Instance.SpawnFromPool(PoolKey.SmallMissile, transform.position + new Vector3(-15.2f, 5 * b - 5, i * 4 - 10), Quaternion.LookRotation(transform.right));
                ObjectPoolManager.Instance.SpawnFromPool(PoolKey.SmallMissile, transform.position + new Vector3(15.2f, 5 * b - 5, i * 4 - 10), Quaternion.LookRotation(-transform.right));
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
           var pulseGun =  ObjectPoolManager.Instance.SpawnFromPool(PoolKey.PulseProjectile, transform.position + new Vector3(5 * i - 5, -15.1f, 0), transform.rotation);
            pulseGun.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            pulseGun.GetComponent<Rigidbody>().AddForce((HW_PlayerStateController.Instance.transform.position - pulseGun.transform.position).normalized * 320, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(0.5f);
        for(int b=0; b<2; b++)
        {
            for (int i = 0; i < 3; i++)
            {
                var pulseGun = ObjectPoolManager.Instance.SpawnFromPool(PoolKey.PulseProjectile, transform.position + new Vector3(5 * i - 5, -15.1f, 10 * b - 5), transform.rotation);
                pulseGun.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                pulseGun.GetComponent<Rigidbody>().AddForce((HW_PlayerStateController.Instance.transform.position - pulseGun.transform.position).normalized *320, ForceMode.Impulse);
            }
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++)
        {
            var pulseGun =  ObjectPoolManager.Instance.SpawnFromPool(PoolKey.PulseProjectile, transform.position + new Vector3(5 * i - 5, -15.1f, 0), transform.rotation);
            pulseGun.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            pulseGun.GetComponent<Rigidbody>().AddForce((HW_PlayerStateController.Instance.transform.position - pulseGun.transform.position).normalized * 320, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(0.5f);
        FireYamato();
    }

    void FireYamato()
    {
        LogManager.Instance.InvokeLine("yamato");
        ObjectPoolManager.Instance.SpawnFromPool(PoolKey.YamatoCannon, transform.position + new Vector3(0, 0, -24), transform.rotation);
    }

    void DroneAttack()
    {
        LogManager.Instance.InvokeLine("drone");
        List<int> randomList = new() { 0, 1, 2, 3, 4, 5 };
        for(int i=0; i<3; i++)
        {
            var drone = ObjectPoolManager.Instance.SpawnFromPool(PoolKey.Drone, transform.position + new Vector3(6 * i - 33, 0, 0), transform.rotation);
            int randomNum = randomList[Random.Range(0, randomList.Count)];
            drone.GetComponent<Drone>().Init(randomNum);
            randomList.Remove(randomNum);
        }
        for (int i = 0; i < 3; i++)
        {
            var drone = ObjectPoolManager.Instance.SpawnFromPool(PoolKey.Drone, transform.position + new Vector3(-6 * i + 33, 0, 0), transform.rotation);
            int randomNum = randomList[Random.Range(0, randomList.Count)];
            drone.GetComponent<Drone>().Init(randomNum);
            randomList.Remove(randomNum);
        }

    }

    void Attack()
    {
        int attackNum = Random.Range(0, 3);
        if (attackNum == 0)
        {
            StartCoroutine(PulseGun());
        }
        else if (attackNum == 1)
        {
            FireSmallMissile();
        }
        else
            DroneAttack();
    }
}
