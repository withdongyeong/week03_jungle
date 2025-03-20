using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MJ_DashState : MJ_IPlayerState
{
    private MJ_PlayerStateController player;

    public void EnterState(MJ_PlayerStateController controller)
    {
        player = controller;
        player.angularSpeed = 0f;
        player.StartCoroutine(Dash());

    }

    public void UpdateState()
    {

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
        
    }

    IEnumerator Dash()
    {
        float time = 0;
        player.rb.linearVelocity = Vector3.zero;
        player.targetdir = player.moveAction.ReadValue<Vector2>();
        player.targetdir = new Vector3(player.targetdir.y, 0, -player.targetdir.x);
        while (time < 1)
        {
            time += Time.deltaTime * 5;
            player.rb.linearVelocity = Vector3.Lerp(player.rb.linearVelocity, player.targetdir * 15, Mathf.Sqrt(time));
            yield return null;
        }
        time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * 3;
            player.rb.linearVelocity = Vector3.Lerp(player.rb.linearVelocity, player.targetdir * 9, Mathf.Sqrt(time));
            yield return null;
        }
        player.ChangeState(new MJ_RunState());
    }


}
