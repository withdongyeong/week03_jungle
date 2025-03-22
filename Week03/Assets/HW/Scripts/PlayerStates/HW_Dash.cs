using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class HW_Dash : IPlayerState
{
    private PlayerMoveManager playerMoveManager;
    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    private Rigidbody rigidBody;
    private CinemachineImpulseSource impulseSource;

    public HW_Dash(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();
        rigidBody = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        playerMoveManager = PlayerMoveManager.Instance;
        playerMoveManager.onGroundedAction += ToRunState;
        impulseSource = playerMoveManager.GetComponent<CinemachineImpulseSource>();
    }

    float dashTurnTime = 0.2f; // 회전 지연 시간
    float dashElapsedTime = 0f;
    float dashTime = 0.2f;
    float dashForce = 160000f; // 각도로 받는 점프력
    float dashAngleY = 5f; // 힘을 받는 각도
    float dashEndForce = 23000f;
    bool dashEnd = false;
    Vector3 finalDashDirection;

    GameObject DashParticle;

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
        float yComponent = Mathf.Sin(dashAngleY * Mathf.Deg2Rad); // 약 0.2588
        Vector3 dashDirection = horizontalDirection; // 수평 방향 복사
        dashDirection.y = yComponent; // Y 성분을 양수로 고정
        finalDashDirection = dashDirection.normalized;

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(); // 기본 설정으로 흔들림
        }

        // 즉시 회전 대신 코루틴으로 dashTurnTime 이후에 회전
        playerMoveManager.StartCoroutine(RotateToDashDirection());

        DashParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/DashParticle"), playerMoveManager.transform);

        ControlLogManager.Instance.SetControlLogText(new List<(int keyboardSpriteIndex, int controllerSpriteIndex, string actionText)>());

        //게임패드 진동

        playerMoveManager.StartVibration();

    }


    // dashTurnTime 동안 부드럽게 회전하는 코루틴
    private IEnumerator RotateToDashDirection()
    {
        Quaternion startRotation = rigidBody.rotation; // 현재 회전
        Quaternion targetRotation = Quaternion.LookRotation(finalDashDirection); // 목표 회전
        float elapsedTime = 0f;

        while (elapsedTime < dashTurnTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dashTurnTime;
            rigidBody.MoveRotation(Quaternion.Slerp(startRotation, targetRotation, t)); // 부드럽게 보간
            yield return null;

            playerMoveManager.MoveByImpulse(finalDashDirection * dashForce * Time.deltaTime);
        }

        // 최종 회전 강제 설정 (보간 오차 방지)
        rigidBody.MoveRotation(targetRotation);
    }

    private void ToRunState()
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_Run(controller));
    }

    public void ExitState()
    {
        playerMoveManager.ManageDashBool(false);

        GameObject.Destroy(DashParticle);
    }

    public void UpdateState()
    {

    }

    public void FixedUpdateState()
    {
        if (!dashEnd && finalDashDirection != null)
        {
            dashElapsedTime += Time.deltaTime;



            if (dashElapsedTime > dashTime)
            {
                dashEnd = true;
                playerMoveManager.MoveByImpulse(-finalDashDirection * dashEndForce);
            }
        }
    }
}