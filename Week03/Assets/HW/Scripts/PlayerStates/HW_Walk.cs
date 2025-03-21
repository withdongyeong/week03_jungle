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
    float walkForce = 600f;
    float maxWalkSpeed = 15f;
    float walkJumpForce = 5000f;
    float normalRotationSpeed = 5f; // �⺻ ȸ�� �ӵ�
    float fastRotationSpeed = 15f; // ���� �ڵ��ƺ��� �ӵ�
    float fastRotationThreshold = 0.7f; // ���� �Է� ���� �Ӱ谪 (�� 90��)

    public void EnterState()
    {
        if(rb.linearVelocity.magnitude > 15)
        {
            rb.linearVelocity = rb.linearVelocity / rb.linearVelocity.magnitude * 15;
        }

        Debug.Log("Entering WalkState");
        actions.Player.Run.performed += ToRunState;
        actions.Player.Attack.performed += ToDashState;
        actions.Player.Jump.performed += ToAirState;
        actions.Player.Move.Enable(); // Move �׼� Ȱ��ȭ ����
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
        actions.Player.Run.performed -= ToRunState;
        actions.Player.Attack.performed -= ToDashState;
        actions.Player.Jump.performed -= ToAirState;
    }

    public void UpdateState()
    {
        Vector2 moveVector = actions.Player.Move.ReadValue<Vector2>();
        if (moveVector.magnitude >= 0.1f || rb.linearVelocity.magnitude >= 0.1f)
        {
            // ī�޶� ���� ���� ���
            Transform cameraTransform = Camera.main.transform;
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            Vector3 moveDirection = (cameraForward * moveVector.y + cameraRight * moveVector.x).normalized;

            // �� ����
            PlayerMoveManager.Instance.MoveByForce(moveDirection * walkForce);

            // ĳ���� ���� ����
            if (moveVector.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                float rotationSpeed = normalRotationSpeed;

                // ���� �Է� ����
                Vector3 currentForward = rb.transform.forward;
                float dotProduct = Vector3.Dot(currentForward, moveDirection);
                bool isBackwardTurn = dotProduct < -fastRotationThreshold;

                if (isBackwardTurn)
                {
                    rotationSpeed = fastRotationSpeed;
                }

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
            }

            // �ӵ� ���� (�ε巯�� ����)
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (flatVelocity.magnitude > maxWalkSpeed)
            {
                // ���� �ӵ� ����
                float decelerationRate = 5f; // �ʴ� ���� ���� (���� ����)
                Vector3 targetVelocity = flatVelocity.normalized * maxWalkSpeed;
                flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, Time.deltaTime * decelerationRate);
                rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);
            }
        }
    }
}