using UnityEngine;

using System.Collections;
using Unity.VisualScripting;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;

public class MJ_RunState : MJ_IPlayerState
{
    private MJ_PlayerStateController player;


    public void EnterState(MJ_PlayerStateController controller)
    {
        player = controller;
        Debug.Log("enter run");
        player.angularSpeed = 2f;

    }

    public void UpdateState()
    {
        player.targetdir = player.moveAction.ReadValue<Vector2>();
        player.targetdir = new Vector3(player.targetdir.y, 0, -player.targetdir.x);
        if (player.rb.linearVelocity.magnitude < 9)
        {
            player.power = 800f;
        }

    }

    public void ExitState()
    {
        
    }

    public void OnDash()
    {
        
    }

    public void OnJump()
    {
        
    }

    public void OnMove()
    {
        
    }

    public void OnStop()
    {
        player.StartCoroutine(IdleCoroutine());
    }

    IEnumerator IdleCoroutine()
    {
        for(int i=0; i<3; i++)
        {
            Debug.Log("idleCoroutine");
            if (player.targetdir == Vector3.zero)
                yield return null;
            else
                yield break;
        }
        player.ChangeState(new MJ_IdleState());
    }
  
    
    
}
