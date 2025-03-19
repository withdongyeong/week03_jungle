using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    private IPlayerState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
       
    
    }

    // Update is called once per frame
    private void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(IPlayerState nextState)
    {
 
        if (nextState == currentState)
        {
            return;
        }
        currentState?.ExitState();
        currentState = nextState;
        nextState.EnterState();
        
    }

}
