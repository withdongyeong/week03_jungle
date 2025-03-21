using UnityEngine;

public class DashState : IPlayerState
{
    private YH_PlayerController player;
    public void EnterState()
    {
        player = YH_PlayerController.instance;
        Debug.Log("Enter Dash");
        player.Dash();
    }
    
    public void UpdateState()
    {
        //Run
    }
    
    public void ExitState()
    {
        Debug.Log("Exit Dash");
    }
}
