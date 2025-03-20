using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamepadInputController : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; } // 카메라 회전 입력 추가
    public bool IsRunning { get; private set; }
    public bool IsDashing { get; private set; }

    private DYInputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new DYInputSystem_Actions();
        inputActions.Enable();
    }

    private void Update()
    {
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
        LookInput = inputActions.Player.Look.ReadValue<Vector2>(); // 카메라 조작 입력
        IsRunning = inputActions.Player.Sprint.IsPressed();
        IsDashing = inputActions.Player.Dash.IsPressed();

        if (inputActions.Player.Restart.IsPressed())
        {
            RestartScene();
        }
    }

    private void RestartScene()
    {
        // 입력 시스템 비활성화
        inputActions.Player.Disable();
        inputActions.UI.Disable();

        // 씬 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // 씬 로딩 후 입력 시스템을 다시 활성화
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드된 후 입력 시스템 다시 활성화
        inputActions.Player.Enable();
        inputActions.UI.Enable();

        // 씬 로딩 이벤트 해제 (한 번만 실행되도록)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
