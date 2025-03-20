using UnityEngine;

public interface DYIPlayerState
{
    void EnterState();
    public void UpdateState(Vector2 moveInput, bool isRunning, bool isDashing);
    void ExitState();
}
