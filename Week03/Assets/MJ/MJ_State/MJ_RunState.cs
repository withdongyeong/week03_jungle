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
        player.playerMaxPlaneSpeed = 15f;
    }

    public void UpdateState()
    {
        player.targetdir = player.moveAction.ReadValue<Vector2>();
        player.targetdir = new Vector3(player.targetdir.x, 0, player.targetdir.y);
        if(player.targetdir.z > 0)
        {
            Debug.Log(player.targetdir.z);
        }

 
    }

    public void ExitState()
    {
        
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
        player.StartCoroutine(IdleCoroutine());
    }

    IEnumerator IdleCoroutine()
    {
        for(int i=0; i<150; i++)
        {
            
            if (player.moveAction.ReadValue<Vector2>() == Vector2.zero)
                yield return null;
            else
            {
                Debug.Log(player.targetdir);
                yield break;
            }
                
        }
        player.ChangeState(new MJ_IdleState());
    }
  
    
    
}
