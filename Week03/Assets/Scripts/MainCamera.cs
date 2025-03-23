using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera : MonoBehaviour
{
    UnityEngine.Camera cam;
    Vector2 rotationEuler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<UnityEngine.Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseMove = Mouse.current.delta.ReadValue();
        rotationEuler.x -= mouseMove.y * 0.3f;
        rotationEuler.y += mouseMove.x * 0.3f;
        Debug.Log(mouseMove);
        transform.rotation = Quaternion.Euler(rotationEuler.x, rotationEuler.y, 0f);

    }
}
