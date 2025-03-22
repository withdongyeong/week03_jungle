using UnityEngine;

public class HW_PlayerStateController : MonoBehaviour
{
    public static HW_PlayerStateController Instance => _instance;
    static HW_PlayerStateController _instance;
    //싱글턴 사용.

    private IPlayerState currentState;
    private IPlayerState previousState; // 이전 상태 저장
    private InputSystem_Actions actions; // 입력 시스템 관리

    private void Awake()
    {


        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        actions = new InputSystem_Actions();

        actions.Player.Enable();

        //Instantiate((GameObject)Resources.Load("HW/Camera/FreeLook Camera"));


    }

    private void Start()
    {
        ChangeState(new HW_Walk(this)); //게임 시작 시에는 Idle.


    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }

    public void ChangeState(IPlayerState nextState)
    {
        if (nextState == currentState) return;
        currentState?.ExitState();
        previousState = currentState; // 이전 상태 저장
        currentState = nextState;
        nextState.EnterState();
    }

    // 상태 클래스에서 사용할 입력 액션 제공
    public InputSystem_Actions GetInputActions()
    {
        return actions;
    }

    // 이전 상태 반환 (기본값으로 Idle)
    public IPlayerState GetPreviousState()
    {
        return previousState ?? new HW_Idle(this);
    }

    private void OnDestroy()
    {
        if (currentState != null)
        {
            currentState.ExitState(); // 상태 정리 보장
            currentState = null;
        }
    }

}

