using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : IPlayerState
{
    InputAction moveAction;
    InputAction jumpAction;

    public void EnterState()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    public void ExitState()
    {
        
    }

    public void UpdateState()
    {
        //만약 wasd 입력이 존재하면, walkstate로 전환.
        if (moveAction.ReadValue<Vector2>() != Vector2.zero)
            PlayerStateController.instance.ChangeState(new WalkState());
    }
}
