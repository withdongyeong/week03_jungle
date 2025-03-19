using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    //현재 상태
    private IPlayerState currentState;

    //싱글톤 패턴
    public static PlayerStateController instance;

    //플레이어 입력 받기
    public float horizontalInput; //a,d 입력.
    public float verticalInput; // w,s 입력.

    //플레이어의 속력
    public float linearSpeed;

    //플레이어가 바라보는 방향
    public Vector3 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ChangeState(new IdleState());
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
