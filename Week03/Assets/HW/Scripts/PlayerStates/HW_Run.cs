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
    float normalRotationSpeed = 5f; // �⺻ ȸ�� �ӵ�
    float fastRotationSpeed = 15f; // ���� �ڵ��ƺ��� �ӵ�
    float fastRotationThreshold = 0.7f; // ���� �Է� ���� �Ӱ谪 (�� 90��)

    [Header("Particle")]
    GameObject groundSweepParticle = null;

    public void EnterState()
    {
        Debug.Log("Enter Run");
        actions.Player.Run.performed += ToWalkState;
        actions.Player.Attack.performed += ToDashState;
        actions.Player.Jump.performed += ToAirRunState;

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
        PlayerMoveManager.Instance.MoveByForce(moveDirection * runForce);

        // ĳ���� ������ �̵� ���⿡ ���� (ī�޶� ����)
        Rigidbody rb = PlayerMoveManager.Instance.GetComponent<Rigidbody>();
        if (moveVector.magnitude > 0.1f)
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

            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        }

        // �ӵ� ����
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxRunSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxRunSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }
}

