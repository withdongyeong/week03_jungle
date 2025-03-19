using UnityEngine;
using UnityEngine.InputSystem;

public interface MJ_IPlayerState 
{
    public void EnterState(MJ_PlayerStateController contoroller);
    public void UpdateState();
    public void ExitState();

    public void OnMove();

    public void OnJump();

    public void OnDash();

    public void OnStop();
}
