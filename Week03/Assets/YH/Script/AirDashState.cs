using UnityEngine;

public class AirDashState : MonoBehaviour
{
    private YH_PlayerController player;
    public void EnterState()
    {
        player = YH_PlayerController.instance;
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
