using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class HW_Idle : IPlayerState
{
    private HW_PlayerStateController controller;
    private InputSystem_Actions actions;
    private PlayerMoveManager playerMoveManager;
    private Rigidbody rb;

    public HW_Idle(HW_PlayerStateController controller)
    {
        this.controller = controller;
        this.actions = controller.GetInputActions();
        playerMoveManager = PlayerMoveManager.Instance;
    }

    float idleJumpForce = 4500f;

    public void EnterState()
    {
        

        actions.Player.Move.performed += ToWalkState; //������ -> Walk
        actions.Player.Jump.performed += ToAirState; //���� -> Jump
    }

    private void ToAirState(InputAction.CallbackContext context)
    {
        if(!playerMoveManager.isJumped)
        {
            playerMoveManager.ManageJumpBool(true); //isJumped => True.
            PlayerMoveManager.Instance.MoveByImpulse(Vector3.up * idleJumpForce); //Jump. ���������� ���� ������ �ϴ� ���.
            HW_PlayerStateController.Instance.ChangeState(new HW_Air(controller)); 
        }

    }

    private void ToWalkState(InputAction.CallbackContext context)
    {
        //playerMoveManager.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        HW_PlayerStateController.Instance.ChangeState(new HW_Walk(controller));
    }

    public void ExitState()
    {
        Debug.Log("Exit Idle");

        //�׼� ����.
        actions.Player.Move.performed -= ToWalkState; 
        actions.Player.Jump.performed -= ToAirState;       
    }

    public void UpdateState()
    {
        
    }


}
