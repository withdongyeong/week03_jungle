using UnityEngine;

public class WalkState : IPlayerState
{
    
    public void EnterState()
    {
        
        Debug.Log("Enter Walk");
    }
    
    public void UpdateState()
    {
        
    }
    
    public void ExitState()
    {
        Debug.Log("Exit Walk");
        
    }
}
