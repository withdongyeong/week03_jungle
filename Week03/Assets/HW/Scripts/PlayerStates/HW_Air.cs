using System;
using System.Collections.Generic;
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
        playerMoveManager = PlayerMoveManager.Instance;
        playerMoveManager.onGroundedAction += ToWalkState;
    }

    float maxAirSpeed = 20f;
    float airForce = 350f;
    float airJumpForce = 120f;

    GameObject airJumpParticle;
    bool isJumping = false; // 점프 입력 상태 추적

    private void ToWalkState()
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Walk(controller));
    }

    public void EnterState()
    {
        playerMoveManager.ManageJumpBool(true);

        actions.Player.Attack.performed += ToAirDashState;
        actions.Player.Run.performed += ToAirRunState;

        ControlLogManager.Instance.SetControlLogText(new List<(int keyboardSpriteIndex, int controllerSpriteIndex, string actionText)>
        {
            (190, 227, "호버링"),
            (196, 228, "공중 달리기"),
            (99, 225, "공중 대시")
        });
    }

    private void ToAirRunState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_AirRun(controller));
    }

    private void ToAirDashState(InputAction.CallbackContext context)
    {
        if (playerMoveManager.UseResourceUsingAction(GameInfoManager.Instance.AirDashResourceUsage))
        {
            HW_PlayerStateController.Instance.ChangeState(new HW_AirDash(controller));
        }
    }

    public void ExitState()
    {
        actions.Player.Attack.performed -= ToAirDashState;
        actions.Player.Run.performed -= ToAirRunState;

        // 상태 종료 시 파티클 제거
        if (airJumpParticle != null)
        {
            GameObject.Destroy(airJumpParticle);
            airJumpParticle = null;
        }
    }

    public void UpdateState()
    {

    }

    public void FixedUpdateState()
    {
        Vector2 moveVector = actions.Player.Move.ReadValue<Vector2>();
        if (moveVector.magnitude >= 0.1f) // 입력이 있을 때만 이동
        {
            Transform cameraTransform = Camera.main.transform;
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            Vector3 moveDirection = (cameraForward * moveVector.y + cameraRight * moveVector.x).normalized;

            PlayerMoveManager.Instance.MoveByImpulse(moveDirection * airForce);

            Rigidbody rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f));

            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (flatVelocity.magnitude > maxAirSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * maxAirSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }

        // 공중 점프 입력 처리
        float jumpInput = actions.Player.Jump.ReadValue<float>();
        if (jumpInput > 0.3f)
        {
            if (playerMoveManager.UseResourceUsingAction(GameInfoManager.Instance.AirJumpResourceUsagePerSec * Time.deltaTime))
            {
                PlayerMoveManager.Instance.MoveByImpulse(Vector3.up * airJumpForce);

                // 점프 시작 시 파티클 생성 (한 번만)
                if (!isJumping)
                {
                    if (airJumpParticle != null)
                    {
                        GameObject.Destroy(airJumpParticle); // 기존 파티클 제거
                    }
                    airJumpParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/AirJumpParticle"), playerMoveManager.transform);
                    isJumping = true;
                }
            }
            else
            {
                if (airJumpParticle != null)
                {
                    GameObject.Destroy(airJumpParticle);
                    airJumpParticle = null;
                }
                isJumping = false;
            }

            Gamepad.current?.SetMotorSpeeds(0.5f, 0.5f);
        }
        else if (isJumping) // 점프 입력이 끝나면 파티클 제거
        {
            if (airJumpParticle != null)
            {
                GameObject.Destroy(airJumpParticle);
                airJumpParticle = null;
            }
            isJumping = false;

            Gamepad.current?.SetMotorSpeeds(0f, 0f);
        }
        else
        {
            Gamepad.current?.SetMotorSpeeds(0f, 0f);
        }
    }
}