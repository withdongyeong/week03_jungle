using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class HW_AirDash : IPlayerState
{
    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    private PlayerMoveManager playerMoveManager;
    private Rigidbody rigidBody;
    private CinemachineImpulseSource impulseSource;
    public HW_AirDash(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();
        playerMoveManager = PlayerMoveManager.Instance;
        rigidBody = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        impulseSource = playerMoveManager.GetComponent<CinemachineImpulseSource>();
    }

    #region move Variables
    float airDashElapsedTime = 0f;
    float airDashTime = 0.3f;
    float airDashForce = 170000f; // 각도로 받는 점프력.
    float airDashAngleY = 3f; // 힘을 받는 각도.
    float controlEnableTime = 0.6f;
    float elapsedControlEnableTime = 0;
    float airDashEndForce = 23000f; //끝났을 때 역방향으로 받는 힘.
    float airDashTurnTime = 0.2f;
    bool airDashEnd = false;
    Vector3 finalAirDashDirection;
    #endregion



    GameObject airDashParticle;

    public void EnterState()
    {
        playerMoveManager.ManageJumpBool(true);
        playerMoveManager.ManageDashBool(true);

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
        float yComponent = Mathf.Sin(airDashAngleY * Mathf.Deg2Rad); // 약 0.2588
        Vector3 dashDirection = horizontalDirection; // 수평 방향 복사
        dashDirection.y = yComponent; // Y 성분을 양수로 고정
        finalAirDashDirection = dashDirection.normalized;

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(); // 기본 설정으로 흔들림
        }

        //rigidBody.MoveRotation(Quaternion.LookRotation(finalAirDashDirection));

        airDashParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/DashParticle"), playerMoveManager.transform);

        playerMoveManager.StartCoroutine(RotateToAirDashDirection());
        playerMoveManager.StartVibration();

        ControlLogManager.Instance.SetControlLogText(new List<(int keyboardSpriteIndex, int controllerSpriteIndex, string actionText)>());
    }

    private IEnumerator RotateToAirDashDirection()
    {
        Quaternion startRotation = rigidBody.rotation; // 현재 회전
        Quaternion targetRotation = Quaternion.LookRotation(finalAirDashDirection); // 목표 회전
        float elapsedTime = 0f;

        while (elapsedTime < airDashTurnTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / airDashTurnTime;
            rigidBody.MoveRotation(Quaternion.Slerp(startRotation, targetRotation, t)); // 부드럽게 보간
            playerMoveManager.MoveByImpulse(finalAirDashDirection * airDashForce * Time.deltaTime);
            yield return null;
        }

        // 최종 회전 강제 설정 (보간 오차 방지)
        rigidBody.MoveRotation(targetRotation);
    }

    public void ExitState()
    {
        playerMoveManager.ManageDashBool(false);

        GameObject.Destroy(airDashParticle);
    }

    public void UpdateState()
    {



    }

    public void FixedUpdateState()
    {
        elapsedControlEnableTime += Time.deltaTime;

        if (!airDashEnd && finalAirDashDirection != null)
        {
            airDashElapsedTime += Time.deltaTime;

            if (airDashElapsedTime > airDashTime)
            {
                airDashEnd = true;
                playerMoveManager.MoveByImpulse(-finalAirDashDirection * airDashEndForce);
            }


        }

        if (elapsedControlEnableTime > controlEnableTime)
        {
            HW_PlayerStateController.Instance.ChangeState(new HW_AirRun(controller));
        }
    }
}
