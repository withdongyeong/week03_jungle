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

    private Vector3 direction;

    public Vector3 targetdir = Vector3.forward;

    public float angularSpeed = 0f;

    public float playerMaxPlaneSpeed = 0;

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
        direction = new Vector3(transform.forward.x, 0, transform.forward.z);      
        if(direction != targetdir)
        {
            float angle = Vector3.SignedAngle(direction, targetdir, Vector3.up);
            if (115 >= angle && angle > 2)
                rb.angularVelocity = Vector3.up * Mathf.Deg2Rad * Vector3.SignedAngle(direction, targetdir, Vector3.up) * 2 * angularSpeed;
            else if (-115 <= angle && angle < -2)
                rb.angularVelocity = Vector3.up * Mathf.Deg2Rad * Vector3.SignedAngle(direction, targetdir, Vector3.up) * 2 * angularSpeed;
            //else if (angle > 115 || angle < -115)
            //{
            //    rb.angularVelocity = Vector3.up * Mathf.Deg2Rad * Vector3.SignedAngle(direction, targetdir, Vector3.up) * 5 * angularSpeed;
            //    rb.linearVelocity = rb.linearVelocity.normalized;
            //    if (angle > 2 || angle < -2)
            //        StartCoroutine(Boost());

            //}

            else
            {
                if(targetdir != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(targetdir);
              
                rb.angularVelocity = Vector3.zero;
            }
        }
        rb.AddForce(transform.forward * power * Time.deltaTime);
        rb.linearVelocity = Vector3.Project(rb.linearVelocity, transform.forward);
    }

    public void ChangeState(MJ_IPlayerState nextState)
    {

        if (nextState == currentState)
        {
            return;
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

    public IEnumerator Boost()
    {
        power *= 2.5f;
        yield return new WaitForSeconds(1f);
        power /= 2.5f;
    }

}
