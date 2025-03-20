using UnityEngine;

public class PulseProjectile : MonoBehaviour
{
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.LookAt(HW_PlayerStateController.Instance.transform);
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 100, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
