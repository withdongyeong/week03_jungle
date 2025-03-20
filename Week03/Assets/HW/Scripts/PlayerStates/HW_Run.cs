using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HW_Run : IPlayerState
{

    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    PlayerMoveManager playerMoveManager;

    public HW_Run(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();
        playerMoveManager = PlayerMoveManager.Instance;
    }

    [Header("Run Variables")]
    float maxRunSpeed = 70f;
    float runForce = 1000f;
    float runJumpForce = 5500f;

    [Header("Particle")]
    GameObject groundSweepParticle = null;

    public void EnterState()
    {
        Debug.Log("Enter Run");
        actions.Player.Crouch.performed += ToWalkState;
        actions.Player.Attack.performed += ToDashState;
        actions.Player.Jump.performed += ToAirState;

        //Spawn particle
        groundSweepParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/GroundSweepParticle"), playerMoveManager.gameObject.transform);
    }

    private void ToWalkState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Walk(controller));
    }

    private void ToDashState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Dash(controller));
    }

    private void ToAirState(InputAction.CallbackContext context) //Ground To Air.
    {
        if(!playerMoveManager.isJumped)
        {
            playerMoveManager.ManageJumpBool(true);
            PlayerMoveManager.Instance.MoveByImpulse(Vector3.up * runJumpForce);
            HW_PlayerStateController.Instance.ChangeState(new HW_Air(controller));
        }

    }

    
    public void ExitState()
    {
        Debug.Log("Exit run");
        actions.Player.Crouch.performed -= ToWalkState;
        actions.Player.Attack.performed -= ToDashState;
        actions.Player.Jump.performed -= ToAirState;

        GameObject.Destroy(groundSweepParticle, 0.3f);
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
        PlayerMoveManager.Instance.MoveByForce(moveDirection * runForce);

        // 캐릭터 방향을 이동 방향에 맞춤 (카메라 기준)
        Rigidbody rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        if (moveVector.magnitude > 0.1f) // 입력이 있을 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f));
        }

        // 속도 제한
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxRunSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxRunSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }
}

