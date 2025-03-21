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
    float normalRotationSpeed = 5f; // �⺻ ȸ�� �ӵ�
    float fastRotationSpeed = 15f; // ���� �ڵ��ƺ��� �ӵ�
    float fastRotationThreshold = 0.7f; // ���� �Է� ���� �Ӱ谪 (�� 90��)
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
        if (moveVector.magnitude < 0.1f) return; // �Է��� ������ ����

        // ī�޶� ���� ���� ���
        Transform cameraTransform = Camera.main.transform; // PlayerMoveManager���� ī�޶� ������
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0; // ���� �̵���
        cameraRight.y = 0;
        Vector3 moveDirection = (cameraForward * moveVector.y + cameraRight * moveVector.x).normalized;

        // �� ���� (�ӵ� ����)
        PlayerMoveManager.Instance.MoveByForce(moveDirection * airRunForce);

        // ĳ���� ������ �̵� ���⿡ ���� (ī�޶� ����)
        Rigidbody rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        if (moveVector.magnitude > 0.1f) // �Է��� ���� ���� ȸ��
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            float rotationSpeed = normalRotationSpeed;

            // ���� �Է� ����
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

        // �ӵ� ����
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxAirRunSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxAirRunSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }
}
