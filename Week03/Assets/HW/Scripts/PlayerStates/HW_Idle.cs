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
        

        actions.Player.Move.performed += ToWalkState; //움직임 -> Walk
        actions.Player.Jump.performed += ToAirState; //점프 -> Jump
    }

    private void ToAirState(InputAction.CallbackContext context)
    {
        if(!playerMoveManager.isJumped)
        {
            playerMoveManager.ManageJumpBool(true); //isJumped => True.
            PlayerMoveManager.Instance.MoveByImpulse(Vector3.up * idleJumpForce); //Jump. 단차로인한 공중 이행을 일단 배려.
            HW_PlayerStateController.Instance.ChangeState(new HW_Air(controller)); 
        }

    }

    private void ToWalkState(InputAction.CallbackContext context)
    {
        playerMoveManager.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        HW_PlayerStateController.Instance.ChangeState(new HW_Walk(controller));
    }

    public void ExitState()
    {
        Debug.Log("Exit Idle");

        //액션 제거.
        actions.Player.Move.performed -= ToWalkState; 
        actions.Player.Jump.performed -= ToAirState;       
    }

    public void UpdateState()
    {
        
    }


}
