using System;
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
    float maxAirRunSpeed = 80f;
    float airRunForce = 1000f;
    float normalRotationSpeed = 5f; // 기본 회전 속도
    float fastRotationSpeed = 15f; // 빠른 뒤돌아보기 속도
    float fastRotationThreshold = 0.7f; // 뒤쪽 입력 감지 임계값 (약 90도)
    GameObject airRunParticle = null;

    public void EnterState()
    {
        actions.Player.Attack.performed += ToAirDashState;

        airRunParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/GroundSweepParticle"), playerMoveManager.gameObject.transform);
    }

    private void ToAirDashState(InputAction.CallbackContext context)
    {
        HW_PlayerStateController.Instance.ChangeState(new HW_AirDash(controller));
    }

    public void ExitState()
    {
        actions.Player.Attack.performed -= ToAirDashState;

        GameObject.Destroy(airRunParticle, 0.2f);
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
        PlayerMoveManager.Instance.MoveByForce(moveDirection * airRunForce);

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
    }
}
