using UnityEngine;

public class WalkState : IPlayerState
{
    public void EnterState()
    {
        Debug.Log("Enter Walk state");
    }

    public void ExitState()
    {
        
    }

    public void UpdateState()
    {
        
        if(PlayerStateController.instance.linearSpeed <1)
        {
            PlayerStateController.instance.linearSpeed += Time.deltaTime;
        }
        PlayerStateController.instance.transform.Translate(PlayerStateController.instance.direction 
            * PlayerStateController.instance.linearSpeed * Time.deltaTime);
    }

   
}
