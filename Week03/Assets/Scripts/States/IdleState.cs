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
        //���� wasd �Է��� �����ϸ�, walkstate�� ��ȯ.
        if (moveAction.ReadValue<Vector2>() != Vector2.zero)
            PlayerStateController.instance.ChangeState(new WalkState());
    }
}
