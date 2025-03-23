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
        player.playerMaxPlaneSpeed = 40;

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
        player.isQuickTurn = true;
        while (time < 1)
        {
            time += Time.deltaTime *8;
            player.rb.linearVelocity = Vector3.Lerp(player.rb.linearVelocity, player.targetdir * 30, Mathf.Sqrt(time));
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        time = 0;
        player.angularSpeed = 20f;
        while (time < 1)
        {
            time += Time.deltaTime * 2;
            player.rb.linearVelocity = Vector3.Lerp(player.rb.linearVelocity, player.targetdir * 9, Mathf.Sqrt(time));
            yield return null;
        }
        player.rb.linearVelocity = player.targetdir * 9;
        player.ChangeState(new MJ_RunState());
    }


}
