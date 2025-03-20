using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class HW_Walk : IPlayerState
{
    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    private PlayerMoveManager playerMoveManager;
    private Rigidbody rb;

    public HW_Walk(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();
        rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        playerMoveManager = PlayerMoveManager.Instance;
    }

    [Header("Walk Variables")]
    float walkForce = 300f;
    float initialWalkPower = 10f;
    float maxWalkSpeed = 25f;
    float walkJumpForce = 5000f;
    float normalRotationSpeed = 5f; // 기본 회전 속도
    float fastRotationSpeed = 15f; // 빠른 뒤돌아보기 속도
    float fastRotationThreshold = 0.7f; // 뒤쪽 입력 감지 임계값 (약 90도)

    public void EnterState()
    {
        Debug.Log("Entering WalkState");
        actions.Player.Crouch.performed += ToRunState;
        actions.Player.Attack.performed += ToDashState;
        actions.Player.Jump.performed += ToAirState;
    }

    private void ToAirState(InputAction.CallbackContext context)
    {
        if(!playerMoveManager.isJumped)
        {
            playerMoveManager.ManageJumpBool(true);
            PlayerMoveManager.Instance.MoveByImpulse(Vector3.up * walkJumpForce);
            HW_PlayerStateController.Instance.ChangeState(new HW_Air(controller));
        }

    }

    private void ToDashState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Dash(controller));
    }

    private void ToRunState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Run(controller));
    }

    public void ExitState()
    {
        Debug.Log("Exit Walk");
        actions.Player.Crouch.performed -= ToRunState;
        actions.Player.Attack.performed -= ToDashState;
        actions.Player.Jump.performed -= ToAirState;
    }

    public void UpdateState()
    {
        Vector2 moveVector = actions.Player.Move.ReadValue<Vector2>();
        if (moveVector.magnitude >= 0.1f || rb.linearVelocity.magnitude >= 0.1f)
        {
            // 카메라 기준 방향 계산
            Transform cameraTransform = Camera.main.transform;
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            Vector3 moveDirection = (cameraForward * moveVector.y + cameraRight * moveVector.x).normalized;

            // 힘 적용
            PlayerMoveManager.Instance.MoveByForce(moveDirection * walkForce);

            // 캐릭터 방향 조정
            Rigidbody rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
            if (moveVector.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                float rotationSpeed = normalRotationSpeed;

                // 뒤쪽 입력 감지
                Vector3 currentForward = rb.transform.forward;
                float dotProduct = Vector3.Dot(currentForward, moveDirection);
                bool isBackwardTurn = dotProduct < -fastRotationThreshold;

                if (isBackwardTurn)
                {
                    rotationSpeed = fastRotationSpeed;
                }

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
            }

            // 속도 제한 (부드러운 감속)
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (flatVelocity.magnitude > maxWalkSpeed)
            {
                // 감속 속도 조정
                float decelerationRate = 1f; // 초당 감속 정도 (조정 가능)
                Vector3 targetVelocity = flatVelocity.normalized * maxWalkSpeed;
                flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, Time.deltaTime * decelerationRate);
                rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);
            }
        }
    }
}