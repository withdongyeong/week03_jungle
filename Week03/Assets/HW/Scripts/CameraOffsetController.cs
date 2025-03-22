using UnityEngine;
using Unity.Cinemachine;

public class CameraOffsetController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float speedThreshold = 15f;
    [SerializeField] private float maxSideOffset = 0.3f;
    [SerializeField] private float transitionSpeed = 5f;

    private CinemachineThirdPersonFollow thirdPersonFollow;
    private float defaultSideOffset;
    private float targetSideOffset;

    private void Awake()
    {
        if (cinemachineCamera == null) cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        thirdPersonFollow = cinemachineCamera.GetComponent<CinemachineThirdPersonFollow>();
        if (thirdPersonFollow == null)
        {
            Debug.LogError("Cinemachine3rdPersonFollow component not found!");
            return;
        }
        defaultSideOffset = thirdPersonFollow.CameraSide;
        targetSideOffset = defaultSideOffset;
    }

    private void Update()
    {
        if (thirdPersonFollow == null) return;

        Vector3 flatVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
        float speed = flatVelocity.magnitude;
        float directionX = Mathf.Sign(playerRigidbody.linearVelocity.x);

        if (speed > speedThreshold)
        {
            targetSideOffset = defaultSideOffset + (directionX * maxSideOffset);
            targetSideOffset = Mathf.Clamp01(targetSideOffset);
        }
        else
        {
            targetSideOffset = defaultSideOffset;
        }

        thirdPersonFollow.CameraSide = Mathf.Lerp(thirdPersonFollow.CameraSide, targetSideOffset, Time.deltaTime * transitionSpeed);
    }
}