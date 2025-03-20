using UnityEngine;

public class HW_PlayerStateController : MonoBehaviour
{
    public static HW_PlayerStateController Instance => _instance;
    static HW_PlayerStateController _instance;
    //�̱��� ���.

    private IPlayerState currentState;
    private IPlayerState previousState; // ���� ���� ����
    private InputSystem_Actions actions; // �Է� �ý��� ����

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        // �Է� �ý��� �ʱ�ȭ
        actions = new InputSystem_Actions();
        actions.Player.Enable(); // Player �׼� �� Ȱ��ȭ
    }

    private void Start()
    {
        ChangeState(new HW_Idle(this)); //���� ���� �ÿ��� Idle.
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(IPlayerState nextState)
    {
        if (nextState == currentState) return;
        currentState?.ExitState();
        previousState = currentState; // ���� ���� ����
        currentState = nextState;
        nextState.EnterState();
    }

    // ���� Ŭ�������� ����� �Է� �׼� ����
    public InputSystem_Actions GetInputActions()
    {
        return actions;
    }

    // ���� ���� ��ȯ (�⺻������ Idle)
    public IPlayerState GetPreviousState()
    {
        return previousState ?? new HW_Idle(this);
    }

}

