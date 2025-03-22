using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMoveManager : MonoBehaviour
{
    public static PlayerMoveManager Instance => _instance;
    static PlayerMoveManager _instance;

    //Own Components.
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Rigidbody rigidBody;

    //others'
    [SerializeField] private Transform cameraTransform;
    InputSystem_Actions actions;

    //Actions.
    public Action onGroundedAction;
    float ResourceRecover;

    //MoveVariables.
    public bool isJumped => _isJumped; bool _isJumped;
    public bool isDash => _isDash; bool _isDash;

    float groundedTransitionTime = 0.1f;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        //Singleton 초기화.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        //Component 초기화.
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidBody = GetComponent<Rigidbody>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        actions = GetComponent<HW_PlayerStateController>().GetInputActions();
        Physics.gravity = new Vector3(0, -20.0f, 0); // 기본값은 (0, -9.81, 0)
        Cursor.visible = false;

        _isDash = false;
        ResourceRecover = GameInfoManager.Instance.ResourceRecover;

        actions.Player.Interact.performed += RestartCurrentScene;
        actions.Player.Previous.performed += DecreasePlayerHp;
        actions.Player.Next.performed += IncreaseMineral;
    }

    private void IncreaseMineral(InputAction.CallbackContext context)
    {
        GameInfoManager.Instance.UpdateMineral(30);
    }

    private void DecreasePlayerHp(InputAction.CallbackContext context)
    {
        GameInfoManager.Instance.UpdateHP(-15);
    }

    public void RestartCurrentScene(InputAction.CallbackContext context)
    {
        Debug.Log("RestartCurrentScene triggered");
        actions.Disable(); // 입력 비활성화
        //actions.Player.Reset; // 입력 버퍼 초기화 (선택 사항)
        _isDash = false; // 대시 상태 강제 초기화
        _isJumped = false; // 점프 상태 강제 초기화
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void MoveByForce(Vector3 force) //Walk. ETC
    {
        rigidBody.AddForce(force, ForceMode.Force);
    }

    public void MoveByImpulse(Vector3 impulseForce) //Dash. ETC
    {
        rigidBody.AddForce(impulseForce, ForceMode.Impulse);
    }

    public void MoveByVelocity(Vector3 velocity)
    {
        rigidBody.linearVelocity = velocity;
    }

    private void Update()
    {
        Vector2 moveInput = actions.Player.Move.ReadValue<Vector2>();
        Vector2 lookInput = actions.Player.Look.ReadValue<Vector2>();

        if(!_isJumped && !_isDash)
        {
            GameInfoManager.Instance.UpdateResource(ResourceRecover * Time.deltaTime);
        }


    }

    public void StartVibration()
    {
        Gamepad.current?.SetMotorSpeeds(0.5f, 0.5f); // 좌우 모터 중간 세기로 0.5초 진동
        Invoke(nameof(StopVibration), 0.2f); // 0.5초 후 진동 중지
    }

    private void StopVibration()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f); // 진동 중지
        }
    }



    public Vector3 GetCameraRelativeDirection(float moveX, float moveZ)
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0; // 수평만 고려
        cameraRight.y = 0;
        return (cameraForward * moveZ + cameraRight * moveX).normalized;
    }

    //public void SetVelocity();

    private void OnCollisionEnter(Collision collision)
    {
        if(isJumped && collision.gameObject.CompareTag("Ground"))
        {
            ManageJumpBool(false);
            Invoke("OnGroundActionInvoker", groundedTransitionTime);

            impulseSource.GenerateImpulse(); // 기본 설정으로 흔들림
            StartVibration();
        }
    }

    private void OnGroundActionInvoker()
    {
        onGroundedAction?.Invoke();
    }

    public void ManageJumpBool(bool _isJumped)
    {
        this._isJumped = _isJumped;
    }

    public void ManageDashBool(bool _isDash)
    {
        this._isDash = _isDash;
    }

    public bool UseResourceUsingAction(float resourceUsage)
    {
        float currentResource = GameInfoManager.Instance.Resource;

        if(currentResource >= 0.00001)
        {
            GameInfoManager.Instance.UpdateResource(-resourceUsage);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        if (actions != null)
        {
            actions.Player.Interact.performed -= RestartCurrentScene;
            actions.Player.Previous.performed -= DecreasePlayerHp;
            actions.Player.Next.performed -= IncreaseMineral;
        }
    }

    internal void RestartCurrentScene()
    {
        Debug.Log("RestartCurrentScene triggered");
        actions.Disable(); // 입력 비활성화
        //actions.Player.Reset; // 입력 버퍼 초기화 (선택 사항)
        _isDash = false; // 대시 상태 강제 초기화
        _isJumped = false; // 점프 상태 강제 초기화
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
