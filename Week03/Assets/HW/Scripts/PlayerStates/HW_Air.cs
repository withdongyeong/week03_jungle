using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HW_Air : IPlayerState
{
    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    private PlayerMoveManager playerMoveManager;

    public HW_Air(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();

        //���� �浹 ����. Air -> Walk.
        playerMoveManager = PlayerMoveManager.Instance;
        playerMoveManager.onGroundedAction += ToWalkState;

    }

    float maxAirSpeed = 30f;
    float airForce = 350f;

    private void ToWalkState() //OnGroundedAction�� Ʈ����.
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Walk(controller));
    }


    public void EnterState()
    {
        playerMoveManager.ManageJumpBool(true); //������ ��Ȳ.

        //input action �ʱ�ȭ.
        actions.Player.Attack.performed += ToAirDashState;
        actions.Player.Run.performed += ToAirRunState;
    }

    private void ToAirRunState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_AirRun(controller));
    }

    private void ToAirDashState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_AirDash(controller));
    }

   

    public void ExitState()
    {
        actions.Player.Attack.performed -= ToAirDashState;
        actions.Player.Run.performed -= ToAirRunState;
    }

    public void UpdateState()
    {
        Vector2 moveVector = actions.Player.Move.ReadValue<Vector2>();
        if (moveVector.magnitude < 0.1f) return; // �Է��� ������ ����

        // ī�޶� ���� ���� ���
        Transform cameraTransform = Camera.main.transform; // PlayerMoveManager���� ī�޶� ������
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0; // ���� �̵���
        cameraRight.y = 0;
        Vector3 moveDirection = (cameraForward * moveVector.y + cameraRight * moveVector.x).normalized;

        // �� ���� (�ӵ� ����)
        PlayerMoveManager.Instance.MoveByForce(moveDirection * airForce);

        // ĳ���� ������ �̵� ���⿡ ���� (ī�޶� ����)
        Rigidbody rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        if (moveVector.magnitude > 0.1f) // �Է��� ���� ���� ȸ��
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f));
        }

        // �ӵ� ����
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxAirSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxAirSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

}
