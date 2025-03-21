using UnityEngine;

public class IdleState : IPlayerState
{
    private YH_PlayerController player;
    public void EnterState()
    {
        player = YH_PlayerController.instance;
        Debug.Log("Enter Idle");
    }
    
    public void UpdateState()
    {
        //Walk
        //Air
    }
    
    public void ExitState()
    {
        Debug.Log("Exit Idle");
        
    }
}
