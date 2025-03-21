using UnityEngine;
using UnityEngine.InputSystem;

public class YH_PlayerInputHandler : MonoBehaviour
{
    private YH_PlayerController _playerController;


    private void Awake()
    {
        _playerController = GetComponent<YH_PlayerController>();
    }

    public void OnWalk(InputValue value)
    {
        _playerController.SetWalkState(value.Get<Vector3>().normalized);
    }

    public void OnDash(InputValue value)
    { 
        Debug.Log("Dash!");
        _playerController.SetDashState();
    }
}
