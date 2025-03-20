using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class HW_Dash : IPlayerState
{
    private PlayerMoveManager playerMoveManager;
    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    private Rigidbody rigidBody;

    public HW_Dash(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();
        rigidBody = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        playerMoveManager = PlayerMoveManager.Instance;
        playerMoveManager.onGroundedAction += ToRunState;
    }

    float dashElapsedTime = 0f;
    float dashTime = 0.23f;
    float dashVelocity = 60f; // 각도로 받는 점프력.
    float dashAngleY = 5f; // 힘을 받는 각도.
    float dashEndForce = 10000f;
    bool dashEnd = false;
    Vector3 finalDashDirection;

    GameObject DashParticle;

    public void EnterState()
    {
        //Cinemachine camera impulse source 트리거.
        playerMoveManager.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        playerMoveManager.ManageJumpBool(true);

        Vector2 moveVector = actions.Player.Move.ReadValue<Vector2>();
        if (moveVector.magnitude < 0.1f)
        {
            moveVector = new Vector2(0, 1); // 기본 forward 방향
        }

        // 카메라 기준 방향 계산 (수평)
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        Vector3 horizontalDirection = (cameraForward * moveVector.y + cameraRight * moveVector.x).normalized;

        // Y축 상향 각도 적용
        float yComponent = Mathf.Sin(dashAngleY * Mathf.Deg2Rad); // 약 0.2588
        Vector3 dashDirection = horizontalDirection; // 수평 방향 복사
        dashDirection.y = yComponent; // Y 성분을 양수로 고정
        finalDashDirection = dashDirection.normalized;

        rigidBody.MoveRotation(Quaternion.LookRotation(finalDashDirection));

        DashParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/DashParticle"), playerMoveManager.transform);
    }


    private void ToRunState()    
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Run(controller));
    }

    public void ExitState()
    {
        //throw new System.NotImplementedException();

        GameObject.Destroy(DashParticle);
    }

    public void UpdateState()
    {
        if(!dashEnd && finalDashDirection != null)
        {
            dashElapsedTime += Time.deltaTime;

            playerMoveManager.MoveByVelocity(finalDashDirection * dashVelocity); 

            if(dashElapsedTime > dashTime)
            {
                dashEnd = true;
                playerMoveManager.MoveByImpulse(-finalDashDirection * dashEndForce);
            }
        }


    }
}
