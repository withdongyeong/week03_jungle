using UnityEngine;

public class SmallMisile : MonoBehaviour
{
    float time = 0f;
    Rigidbody rb;
    [SerializeField] private float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (1.8f > time && time > 0)
        {
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime * 13f);

        }
        else if (3.4f > time && time > 1.8f)
        {
            transform.LookAt(HW_PlayerStateController.Instance.transform);

        }
        else if (time > 3.4f)
        {
            rb.linearVelocity = transform.forward * speed * 3.5f;
            Vector3 targetDir = HW_PlayerStateController.Instance.transform.position - transform.position;
            if (Vector3.Angle(transform.forward, targetDir) < 53f)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.Slerp(transform.forward, targetDir, 0.01f));

            }
        }
    }
}

