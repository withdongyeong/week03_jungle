using UnityEngine;

public class IdleState : DYIPlayerState
{
    private Rigidbody rb;
    private float speedTurnStartTime;
    private bool isSpeedTurning = false; // Speed Turn 진행 여부

    public void EnterState()
    {
        rb = DYPlayerStateController.Instance.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero; // 이동 멈춤
    }

    public void UpdateState(Vector2 moveInput, bool isRunning, bool isDashing)
    {
        if (isSpeedTurning)
        {
            // Speed Turn 중이면, 일정 시간이 지난 후에 정상 조작 가능
            if (Time.time - speedTurnStartTime >= GlobalSettings.Instance.SpeedTurnDuration)
            {
                isSpeedTurning = false; // Speed Turn 종료
            }
            else
            {
                return; // Speed Turn 중에는 아무것도 하지 않음
            }
        }

        Vector3 flatMoveInput = new Vector3(moveInput.x, 0, moveInput.y);

        if (flatMoveInput.magnitude > 0.1f)
        {
            // 현재 바라보는 방향과 입력된 방향 사이의 각도 차이 계산
            float angleDiff = Vector3.Angle(rb.transform.forward, flatMoveInput);

            // 급격한 방향 전환이 있을 때 Speed Turn 실행
            if (angleDiff > GlobalSettings.Instance.SpeedTurnThreshold)
            {
                SpeedTurn(flatMoveInput);
                return; // Speed Turn 실행 후 바로 종료
            }

            // 일반적인 회전 처리
            Quaternion targetRotation = Quaternion.LookRotation(flatMoveInput);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * GlobalSettings.Instance.RotationSpeed));
        }

        if (isDashing)
        {
            DYPlayerStateController.Instance.ChangeState<DashState>();
        }
        else if (moveInput.magnitude > 0)
        {
            if (isRunning)
                DYPlayerStateController.Instance.ChangeState<RunState>();
            else
                DYPlayerStateController.Instance.ChangeState<WalkState>();
        }

        // 감속 처리 (입력이 없을 때 점차적으로 속도 감소)
        if (moveInput.magnitude == 0)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * GlobalSettings.Instance.BoosterDeceleration);
        }
    }

    public void ExitState()
    {
    }

    private void SpeedTurn(Vector3 moveDirection)
    {
        isSpeedTurning = true;
        speedTurnStartTime = Time.time; // Speed Turn 시작 시간 기록

        // 캐릭터를 위로 살짝 띄우고 빠르게 회전
        rb.AddForce(Vector3.up * GlobalSettings.Instance.SpeedTurnBoostForce, ForceMode.Impulse);  // 위로 부스터
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.MoveRotation(targetRotation);
    }
}
