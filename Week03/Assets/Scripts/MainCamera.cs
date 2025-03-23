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
        Vector2 mouseMove = Mouse.current.delta.ReadValue();
        Debug.Log(mouseMove);
        transform.Rotate(-mouseMove.y * 0.5f, mouseMove.x * 0.5f, 0);

    }
}
