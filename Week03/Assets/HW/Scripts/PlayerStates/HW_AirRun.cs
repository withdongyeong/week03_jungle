using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HW_AirRun : IPlayerState
{
    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    private PlayerMoveManager playerMoveManager;

    public HW_AirRun(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();
        playerMoveManager = PlayerMoveManager.Instance;
        playerMoveManager.onGroundedAction += ToRunState;
    }

    private void ToRunState()
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Run(controller));
    }

    [Header("Run Variables")]
    float minAirRunSpeed = 5f;
    float maxAirRunSpeed = 40f;
    float airRunForce = 900f;
    float normalRotationSpeed = 5f; // 기본 회전 속도
    float fastRotationSpeed = 15f; // 빠른 뒤돌아보기 속도
    float fastRotationThreshold = 0.7f; // 뒤쪽 입력 감지 임계값 (약 90도)
    float airJumpForce = 300f;
    bool isJumping = false;
    GameObject airRunParticle = null;
    GameObject airJumpParticle;

    public void EnterState()
    {
        actions.Player.Attack.performed += ToAirDashState;
        actions.Player.Run.performed += ToWalkState;

        airRunParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/GroundSweepParticle"), playerMoveManager.gameObject.transform);

        ControlLogManager.Instance.SetControlLogText(new List<(int keyboardSpriteIndex, int controllerSpriteIndex, string actionText)>
        {
            (190, 227, "호버링"),   // 키보드: 인덱스 0, 게임패드: 인덱스 1
            (196, 228, "달리기 해제"), // 키보드: 인덱스 2, 게임패드: 인덱스 3
            (99, 225, "공중 대시")    // 키보드: 인덱스 4, 게임패드: 인덱스 5
        });
        
    }

    private void ToWalkState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Air(controller));
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

        // 상태 종료 시 파티클 제거
        if (airJumpParticle != null)
        {
            GameObject.Destroy(airJumpParticle);
            airJumpParticle = null;
        }

        GameObject.Destroy(airRunParticle, 0.2f);
    }

    public void UpdateState()
    {

    }


    private void ToAirState()
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Air(controller));
    }

    public void FixedUpdateState()
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
        PlayerMoveManager.Instance.MoveByImpulse(moveDirection * airRunForce);

        // 캐릭터 방향을 이동 방향에 맞춤 (카메라 기준)
        Rigidbody rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        if (moveVector.magnitude > 0.1f) // 입력이 있을 때만 회전
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
                GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/StoppingParticle"), playerMoveManager.gameObject.transform.position, playerMoveManager.gameObject.transform.rotation);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f));
                playerMoveManager.StartVibration();
                ToAirState();
            }

            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f));

        }

        // 속도 제한
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxAirRunSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxAirRunSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
        else if (flatVelocity.magnitude < minAirRunSpeed)
        {
            ToAirState();
        }

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



