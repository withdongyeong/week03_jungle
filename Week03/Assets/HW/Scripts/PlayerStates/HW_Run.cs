using System;
using System.Collections.Generic;
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
    float maxRunSpeed = 35f;
    float minRunSpeed = 2f;
    float runForce = 800f;
    float runJumpForce = 3500f;
    float normalRotationSpeed = 5f; // 기본 회전 속도
    float fastRotationSpeed = 15f; // 빠른 뒤돌아보기 속도
    float fastRotationThreshold = 0.7f; // 뒤쪽 입력 감지 임계값 (약 90도)

    [Header("Particle")]
    GameObject groundSweepParticle = null;

    public void EnterState()
    {
        Debug.Log("Enter Run");
        actions.Player.Run.performed += ToWalkState;
        actions.Player.Attack.performed += ToDashState;
        actions.Player.Jump.performed += ToAirRunState;

        playerMoveManager.ManageJumpBool(false);

        //Spawn particle
        groundSweepParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/GroundSweepParticle"), playerMoveManager.gameObject.transform);

        ControlLogManager.Instance.SetControlLogText(new List<(int keyboardSpriteIndex, int controllerSpriteIndex, string actionText)>
        {
            (190, 227, "점프"),   // 키보드: 인덱스 0, 게임패드: 인덱스 1
            (196, 228, "달리기 해제"), // 키보드: 인덱스 2, 게임패드: 인덱스 3
            (99, 225, "대시")    // 키보드: 인덱스 4, 게임패드: 인덱스 5
        });
        
    }

    private void ToWalkState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Walk(controller));
    }

    private void ToDashState(InputAction.CallbackContext context)
    {
        if(playerMoveManager.UseResourceUsingAction(GameInfoManager.Instance.DashResourceUsage))
        {
            HW_PlayerStateController.Instance.ChangeState(new HW_Dash(controller));
        }

        
    }

    private void ToAirRunState(InputAction.CallbackContext context) //Ground To Air.
    {
        if(!playerMoveManager.isJumped)
        {
            playerMoveManager.ManageJumpBool(true);
            PlayerMoveManager.Instance.MoveByImpulse(Vector3.up * runJumpForce);
            HW_PlayerStateController.Instance.ChangeState(new HW_AirRun(controller));
        }

    }

    
    public void ExitState()
    {
        Debug.Log("Exit run");
        actions.Player.Run.performed -= ToWalkState;
        actions.Player.Attack.performed -= ToDashState;
        actions.Player.Jump.performed -= ToAirRunState;

        GameObject.Destroy(groundSweepParticle, 0.1f);
        Gamepad.current?.SetMotorSpeeds(0f, 0f);
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

                GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/StoppingParticle"), playerMoveManager.gameObject.transform.position, playerMoveManager.gameObject.transform.rotation);
                playerMoveManager.StartVibration();
                ToWalkState();
            }

            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        }

        // 속도 제한
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxRunSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxRunSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
        else if(flatVelocity.magnitude < minRunSpeed)
        {
            ToWalkState();
        }
            
        Gamepad.current?.SetMotorSpeeds(0.1f, 0.1f);
    }

    private void ToWalkState()
    {

        HW_PlayerStateController.Instance.ChangeState(new HW_Walk(controller));
    }
}

