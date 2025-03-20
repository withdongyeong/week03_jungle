using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Transform playerBody;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private GamepadInputController inputController;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerBody = playerObject.transform;
        }
        else
        {
            Debug.LogError("CameraController: No GameObject with tag 'Player' found!");
        }

        inputController = FindAnyObjectByType<GamepadInputController>();
    }

    private void LateUpdate()
    {
        if (playerBody == null) return;

        Vector2 lookInput = inputController.LookInput;
        bool isUsingGamepad = Gamepad.current != null;
        float sensitivity = isUsingGamepad ? GlobalSettings.Instance.GamepadSensitivity : GlobalSettings.Instance.MouseSensitivity;

        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, GlobalSettings.Instance.YRotationLimit.x, GlobalSettings.Instance.YRotationLimit.y);

        // 카메라 위치를 설정하는 부분에서 카메라 거리 조정
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.position = playerBody.position - (rotation * Vector3.forward * GlobalSettings.Instance.CameraDistance);  // 거리 늘림
        transform.LookAt(playerBody.position + Vector3.up * 1.5f);
    }
}
