using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MJ_PlayerStateController : MonoBehaviour
{
    //현재 상태
    private MJ_IPlayerState currentState;

    public Rigidbody rb { get; private set; }


    //인풋 매니저와 관련된 변수들.
    public InputAction moveAction { get; private set; }

    public float power;

    public float turnSpeed;

    public Vector3 direction;

    public Vector3 targetdir = Vector3.forward;

    public float angularSpeed = 0f;

    public float playerMaxPlaneSpeed = 0;

    public bool isQuickTurn;

    public Transform cameraTransform;
    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.canceled += OnStop;
        rb = GetComponent<Rigidbody>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeState(new MJ_IdleState());
    }

    // Update is called once per frame
    void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(MJ_IPlayerState nextState)
    {
        if(currentState!=null)
        {
            if (nextState.GetType() == currentState.GetType())
            {
                return;
            }
        }
        currentState?.ExitState();
        currentState = nextState;
        nextState.EnterState(this);

    }

    public void OnMove()
    {
        currentState?.OnMove();
    }

    public void OnStop(InputAction.CallbackContext context)
    {
        currentState?.OnStop();
    }

    public void OnDash()
    {
        currentState?.OnDash();
    }

    private void SpeedChange()
    {
        float speedDif = targetdir.x * playerMaxPlaneSpeed - rb.linearVelocity.x;
        if(Mathf.Abs(speedDif) < 0.2f)
        {
            speedDif = 0.2f * Mathf.Sign(speedDif);
        }
        rb.linearVelocity += new Vector3(speedDif * Time.deltaTime * 0.5f, 0, 0);  
    }



}
