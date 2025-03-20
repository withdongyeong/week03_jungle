using System.Collections;
using UnityEngine;

public class BattleCruser : MonoBehaviour
{
    Transform playerTransform;
    [SerializeField] private GameObject smallMissile;
    [SerializeField] private GameObject pulseGun;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = HW_PlayerStateController.Instance.transform;
        Invoke("ComboAttack", 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FireSmallMissile()
    {
        for(int b=0; b<3; b++)
        {
            for (int i = 0; i < 6; i++)
            {
                Instantiate(smallMissile, transform.position + new Vector3(-15.1f, 5 * b - 5, i * 4 - 10), Quaternion.LookRotation(-transform.right));
                Instantiate(smallMissile, transform.position + new Vector3(15.1f, 5 * b - 5, i * 4 - 10), Quaternion.LookRotation(transform.right));
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
            Instantiate(pulseGun, transform.position + new Vector3(5 * i - 5, -15.1f, 0), transform.rotation);
        }
        yield return new WaitForSeconds(0.5f);
        for(int b=0; b<2; b++)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(pulseGun, transform.position + new Vector3(5 * i - 5, -15.1f, 10*b - 5), transform.rotation);
            }
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++)
        {
            Instantiate(pulseGun, transform.position + new Vector3(5 * i - 5, -15.1f, 0), transform.rotation);
        }
    }
}
