using UnityEngine;

public class DashState : DYIPlayerState
{
    private Rigidbody rb;
    private float dashStartTime;

    public void EnterState()
    {
        rb = DYPlayerStateController.Instance.GetComponent<Rigidbody>();

        Dash();
        dashStartTime = Time.time; // Dash 시작 시간 기록
    }

    public void UpdateState(Vector2 moveInput, bool isRunning, bool isDashing)
    {
        // Dash 상태가 끝나면 Run 상태로 전환
        if (Time.time - dashStartTime >= GlobalSettings.Instance.DashDuration)
        {
            DYPlayerStateController.Instance.ChangeState<RunState>();
        }
    }

    public void ExitState()
    {
    }

    private void Dash()
    {
        // Dash를 위한 방향 벡터 계산
        Vector3 dashDirection = rb.transform.forward * GlobalSettings.Instance.DashForwardForce
                                + Vector3.up * GlobalSettings.Instance.DashUpForce;

        // Dash 효과
        rb.AddForce(dashDirection, ForceMode.Impulse);
    }
}
