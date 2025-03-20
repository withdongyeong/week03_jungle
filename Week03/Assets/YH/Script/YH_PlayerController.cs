using System;
using UnityEngine;

public class YH_PlayerController : MonoBehaviour
{
    public int rbIDTEST;
    
    
    public static YH_PlayerController instance;
    [SerializeField]
    private IPlayerState currentState;
    [SerializeField]
    private float speed;
    [SerializeField] 
    private float dashPower = 30f;
    [SerializeField]
    private Vector3 input;
    [SerializeField]
    private float walkSpeed; //확인용
    [SerializeField]
    private float maxWalkSpeed;
    [SerializeField]
    private float maxRunSpeed;
    private Rigidbody rb;
    public float maxboost;
    public float boost;
    public Vector3 power;
    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        currentState = new IdleState();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        currentState?.UpdateState();
        walkSpeed = rb.linearVelocity.magnitude;
    }

    private void FixedUpdate()
    {
        UpdateWalk();
    }

    public void ChangeState(IPlayerState nextState)
    {
        Debug.Log("ChangeState" + currentState.ToString());
        Debug.Log(nextState.GetType() + " " + currentState.GetType());
        if (nextState.GetType() == currentState.GetType())
        {
            return;
        }
        currentState?.ExitState();
        currentState = nextState;
        nextState.EnterState();
        
    }

    public void SetWalkState(Vector3 inputVector)
    {
        input = inputVector;
        if (input != Vector3.zero)
        {
            ChangeState(new WalkState());
        }
        else if(rb.linearVelocity.magnitude < 0.1f)
        {
            ChangeState(new IdleState());
        }
    }

    public void SetDashState()
    {
        if(currentState is WalkState)
        {
            ChangeState(new DashState());
        }
    }

    public void Dash()
    {
        power = (speed*dashPower)*input;
        rb.AddForce(power , ForceMode.Force); //질량 무관 사용중
    }
    
    
    private void UpdateWalk()
    {
        if(rb.linearVelocity.magnitude > maxWalkSpeed) return;
        Vector3 inputVector = new Vector3(input.x, 0, input.z);
        //transform.rotation = Quaternion.LookRotation(inputVector);
        power = (speed + boost*100)*input;
        rb.AddForce(power , ForceMode.Force); //질량 무관 사용중
        if(input != Vector3.zero)
        {
            boost += Time.deltaTime * maxboost;
        }
        else
        {
            boost = 0;
        }
    }
}
