using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance { get; private set; }

    [Header("Movement Settings")]
    private float walkForce = 5f;
    private float runForce = 10f;
    private float dashForwardForce = 30f;
    private float dashUpForce = 4f;
    private float dashDuration = 1f;
    private float boosterDeceleration = 1f;
    private float rotationSpeed = 2f;
    private float speedTurnThreshold = 170f;
    private float speedTurnBoostForce = 10f;
    private float speedTurnSpeed = 30f;
    private float speedTurnDuration = 0.3f;
    private float maxWalkSpeed = 8f;
    private float maxRunSpeed = 15f;

    [Header("Camera Settings")]
    private float mouseSensitivity = 10f;
    private float gamepadSensitivity = 75f;
    private Vector2 yRotationLimit = new Vector2(-30f, 75f);
    private float cameraDistance = 5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Movement settings getters
    public float WalkForce => walkForce;
    public float RunForce => runForce;
    public float DashForwardForce => dashForwardForce;
    public float DashUpForce => dashUpForce;
    public float DashDuration => dashDuration;
    public float BoosterDeceleration => boosterDeceleration;
    public float RotationSpeed => rotationSpeed;
    public float SpeedTurnThreshold => speedTurnThreshold;
    public float SpeedTurnDuration => speedTurnDuration;
    public float MaxRunSpeed => maxRunSpeed;
    public float MaxWalkSpeed => maxWalkSpeed;

    // Camera settings getters
    public float MouseSensitivity => mouseSensitivity;
    public float GamepadSensitivity => gamepadSensitivity;
    public Vector2 YRotationLimit => yRotationLimit;
    public float CameraDistance => cameraDistance;
    public float SpeedTurnBoostForce => speedTurnBoostForce;
    public float SpeedTurnSpeed => speedTurnSpeed;
}
