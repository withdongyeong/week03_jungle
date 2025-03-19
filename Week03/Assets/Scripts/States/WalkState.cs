using UnityEngine;

public class WalkState : IPlayerState
{
    public void EnterState()
    {
        Debug.Log("hello world");
    }

    public void ExitState()
    {
        Debug.Log("exit");
    }

    public void UpdateState()
    {
        Debug.Log("update complete");
    }

   
}
