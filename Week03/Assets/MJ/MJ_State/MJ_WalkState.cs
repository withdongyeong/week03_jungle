
using UnityEngine;
using UnityEngine.InputSystem;

public class MJ_WalkState : MJ_IPlayerState
{
    private MJ_PlayerStateController player;
    public void EnterState(MJ_PlayerStateController controller)
    {
        player = controller;
        InputSystem.actions.FindAction("Interact").started += OnRun;
        Debug.Log("enter walk");
        player.angularSpeed = 3;
    }

    public void UpdateState()
    {
        player.targetdir = player.moveAction.ReadValue<Vector2>();
        player.targetdir = new Vector3(player.targetdir.y, 0, -player.targetdir.x);
        if (player.rb.linearVelocity.magnitude < 3)
        {
            player.power = 100f;
        }
        else
            player.power = 0f;


    }

    public void ExitState()
    {
        InputSystem.actions.FindAction("Interact").started -= OnRun;
    }

    public void OnDash()
    {
        player.ChangeState(new MJ_DashState());
    }

    public void OnJump()
    {
       
    }

    public void OnMove()
    {
        
    }

    public void OnStop()
    {
        player.ChangeState(new MJ_IdleState());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        player.ChangeState(new MJ_RunState());
    }
   


}
