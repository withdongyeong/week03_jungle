using UnityEngine;

public class RunState : IPlayerState
{
    private YH_PlayerController player;
    public void EnterState()
    {
        player = YH_PlayerController.instance;
        Debug.Log("Enter Run");
    }
    
    public void UpdateState()
    {
        //Idle
        //Walk
        //Dash
        //Air
    }
    
    public void ExitState()
    {
        Debug.Log("Exit Run");
        
    }
}
