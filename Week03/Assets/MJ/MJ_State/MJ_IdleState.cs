using UnityEngine;
using UnityEngine.InputSystem;

public class MJ_IdleState : MJ_IPlayerState
{
    private MJ_PlayerStateController player;
    public void EnterState(MJ_PlayerStateController controller)
    {
        player = controller;
        Debug.Log("enter idle");
        player.power = 0f;
        player.maxSpeed = 0f;
    }

    public void UpdateState()
    {
      
    }

    public void ExitState()
    {
        Debug.Log("exit idle");
    }

    public void OnDash()
    {

    }

    public void OnJump()
    {

    }

    public void OnMove()
    {
        player.ChangeState(new MJ_WalkState());
    }

    public void OnStop()
    {
        
    }

    private void SpeedChange()
    {

    }
}
