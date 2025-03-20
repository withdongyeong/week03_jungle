using UnityEngine;

public class DYPlayerStateController : MonoBehaviour
{
    public static DYPlayerStateController Instance { get; private set; }

    private DYIPlayerState currentState;
    private DYIPlayerState idleState;
    private DYIPlayerState walkState;
    private DYIPlayerState runState;
    private DYIPlayerState dashState;

    private GamepadInputController inputController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (inputController == null)
        {
            GameObject inputObj = new GameObject("GamepadInputController");
            inputController = inputObj.AddComponent<GamepadInputController>();
        }
    }


    private void Start()
    {
        idleState = new IdleState();
        walkState = new WalkState();
        runState = new RunState();
        dashState = new DashState();

        currentState = idleState;
        currentState.EnterState();
    }

    private void Update()
    {
        Debug.Log(currentState);

        // 1. 원래 입력값 가져오기
        Vector2 rawMoveInput = inputController.MoveInput;
        bool isRunning = inputController.IsRunning;
        bool isDashing = inputController.IsDashing;

        // 2. 카메라 기준 이동 방향 변환
        Vector3 moveDirection = ConvertInputToCameraRelativeDirection(rawMoveInput);

        // 3. 변환된 방향을 상태에 전달
        currentState.UpdateState(new Vector2(moveDirection.x, moveDirection.z), isRunning, isDashing);
    }

    // ✅ 카메라 기준으로 이동 입력 변환 함수
    private Vector3 ConvertInputToCameraRelativeDirection(Vector2 moveInput)
    {
        if (Camera.main == null) return Vector3.zero;

        Transform cameraTransform = Camera.main.transform;

        // 카메라의 forward와 right 벡터 가져오기
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // Y축 제거 (수평 이동만 필요함)
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 입력 벡터를 카메라 방향에 맞게 변환
        return (camForward * moveInput.y + camRight * moveInput.x).normalized;
    }


    public void ChangeState<T>() where T : class, DYIPlayerState
    {
        if (typeof(T) == currentState.GetType()) return;

        currentState.ExitState();

        switch (typeof(T).Name)
        {
            case nameof(IdleState):
                currentState = idleState;
                break;
            case nameof(WalkState):
                currentState = walkState;
                break;
            case nameof(RunState):
                currentState = runState;
                break;
            case nameof(DashState):
                currentState = dashState;
                break;
            default:
                Debug.LogWarning($"ChangeState: {typeof(T).Name} is not a valid state.");
                return;
        }

        currentState.EnterState();
    }
}
