using System;
using UnityEngine;

public class HW_AirDash : IPlayerState
{
    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    private PlayerMoveManager playerMoveManager;

    public HW_AirDash(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();
        playerMoveManager = PlayerMoveManager.Instance;
    }

    float airDashElapsedTime = 0f;
    float airDashTime = 0.3f;
    float airDashVelocity = 50f; // ������ �޴� ������.
    float airDashAngleY = 3f; // ���� �޴� ����.
    float controlEnableTime = 0.6f;
    float elapsedControlEnableTime = 0;
    float airDashEndForce = 10000f; //������ �� ���������� �޴� ��.
    bool airDashEnd = false;
    Vector3 finalAirDashDirection;

    GameObject airDashParticle;

    public void EnterState()
    {
        playerMoveManager.ManageJumpBool(true);

        Vector2 moveVector = actions.Player.Move.ReadValue<Vector2>();
        if (moveVector.magnitude < 0.1f)
        {
            moveVector = new Vector2(0, 1); // �⺻ forward ����
        }

        // ī�޶� ���� ���� ��� (����)
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        Vector3 horizontalDirection = (cameraForward * moveVector.y + cameraRight * moveVector.x).normalized;

        // Y�� ���� ���� ����
        float yComponent = Mathf.Sin(airDashAngleY * Mathf.Deg2Rad); // �� 0.2588
        Vector3 dashDirection = horizontalDirection; // ���� ���� ����
        dashDirection.y = yComponent; // Y ������ ����� ����
        finalAirDashDirection = dashDirection.normalized;

        airDashParticle = GameObject.Instantiate((GameObject)Resources.Load("HW/Particle/DashParticle"), playerMoveManager.transform);
    }

    public void ExitState()
    {
        GameObject.Destroy(airDashParticle);
    }

    public void UpdateState()
    {
        elapsedControlEnableTime += Time.deltaTime;

        if (!airDashEnd && finalAirDashDirection != null)
        {
            airDashElapsedTime += Time.deltaTime;

            playerMoveManager.MoveByVelocity(finalAirDashDirection * airDashVelocity);

            if (airDashElapsedTime > airDashTime)
            {
                airDashEnd = true;
                playerMoveManager.MoveByImpulse(-finalAirDashDirection * airDashEndForce);
            }

            
        }

        if(elapsedControlEnableTime > controlEnableTime)
        {
            HW_PlayerStateController.Instance.ChangeState(new HW_AirRun(controller));
        }


    }
}
