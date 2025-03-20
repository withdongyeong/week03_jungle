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

        //지면 충돌 감지. Air -> Walk.
        playerMoveManager = PlayerMoveManager.Instance;
        playerMoveManager.onGroundedAction += ToWalkState;

    }

    float maxAirSpeed = 30f;
    float airForce = 350f;

    private void ToWalkState() //OnGroundedAction이 트리거.
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Walk(controller));
    }


    public void EnterState()
    {
        playerMoveManager.ManageJumpBool(true); //점프한 상황.

        //input action 초기화.
        actions.Player.Attack.performed += ToAirDashState;
        actions.Player.Crouch.performed += ToAirRunState;
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
        actions.Player.Crouch.performed -= ToAirRunState;
    }

    public void UpdateState()
    {
        Vector2 moveVector = actions.Player.Move.ReadValue<Vector2>();
        if (moveVector.magnitude < 0.1f) return; // 입력이 없으면 종료

        // 카메라 기준 방향 계산
        Transform cameraTransform = Camera.main.transform; // PlayerMoveManager에서 카메라 가져옴
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0; // 수평 이동만
        cameraRight.y = 0;
        Vector3 moveDirection = (cameraForward * moveVector.y + cameraRight * moveVector.x).normalized;

        // 힘 적용 (속도 조절)
        PlayerMoveManager.Instance.MoveByForce(moveDirection * airForce);

        // 캐릭터 방향을 이동 방향에 맞춤 (카메라 기준)
        Rigidbody rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        if (moveVector.magnitude > 0.1f) // 입력이 있을 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f));
        }

        // 속도 제한
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxAirSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxAirSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

}
