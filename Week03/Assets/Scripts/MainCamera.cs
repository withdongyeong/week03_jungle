using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera : MonoBehaviour
{
    UnityEngine.Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<UnityEngine.Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(transform.forward, transform.position + transform.forward * 2);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance); // ���콺�� ���ϴ� ���� ��ġ
            Vector3 lookDir = (point - transform.position).normalized;
            lookDir.y = 0f; // ���� ȸ���� �ϰ� ������ y�� ����

            if (lookDir != Vector3.zero)
            {
                transform.forward = lookDir;
            }
        }
    }
}
