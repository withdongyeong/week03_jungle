using System;
using UnityEngine;

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
    }

    private void Start()
    {
        actions = GetComponent<HW_PlayerStateController>().GetInputActions();
        Physics.gravity = new Vector3(0, -20.0f, 0); // 기본값은 (0, -9.81, 0)
        Cursor.visible = false;

        _isDash = false;
        ResourceRecover = GameInfoManager.Instance.ResourceRecover;
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

        if(currentResource >= 1)
        {
            GameInfoManager.Instance.UpdateResource(-resourceUsage);

            return true;
        }
        else
        {
            return false;
        }
    }
}
