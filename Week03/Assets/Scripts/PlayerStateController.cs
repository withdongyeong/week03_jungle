using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    //���� ����
    private IPlayerState currentState;

    //�̱��� ����
    public static PlayerStateController instance;

    //�÷��̾� �Է� �ޱ�
    public float horizontalInput; //a,d �Է�.
    public float verticalInput; // w,s �Է�.

    //�÷��̾��� �ӷ�
    public float linearSpeed;

    //�÷��̾ �ٶ󺸴� ����
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
