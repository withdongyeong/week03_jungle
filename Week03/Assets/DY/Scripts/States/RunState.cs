using UnityEngine;

public class RunState : DYIPlayerState
{
    private Rigidbody rb;
    private float speedTurnStartTime;
    private bool isSpeedTurning = false; // Speed Turn 진행 여부

    public void EnterState()
    {
        rb = DYPlayerStateController.Instance.GetComponent<Rigidbody>();
        // Run 상태에서 부드러운 감속을 위해 drag 값 조정 (일정 비율로 줄어들도록)
        rb.linearDamping = 1f;  // Run 상태에서 자연스러운 감속을 위해 drag 값을 설정
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

            // 급격한 방향 전환이 있을 때
            if (angleDiff > GlobalSettings.Instance.SpeedTurnThreshold)
            {
                // 방향 전환을 빠르게 적용
                SpeedTurn(flatMoveInput);
            }
            else
            {
                // 일반적인 회전 처리
                Quaternion targetRotation = Quaternion.LookRotation(flatMoveInput);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * GlobalSettings.Instance.RotationSpeed));
            }
        }

        if (flatMoveInput.magnitude > 0.1f) // 적당한 threshold로 회전값 적용
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatMoveInput);  // x, z값만 사용하여 회전
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 0.1f));  // 회전
        }
        if (isDashing)
        {
            DYPlayerStateController.Instance.ChangeState<DashState>();
            return;
        }
        if (!isRunning)
        {
            DYPlayerStateController.Instance.ChangeState<WalkState>();
            return;
        }
        if (moveInput.magnitude == 0)
        {
            DYPlayerStateController.Instance.ChangeState<IdleState>();
            return;
        }

        // 이동 방향 벡터 계산
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // AddForce() 적용
        rb.AddForce(moveDirection * GlobalSettings.Instance.RunForce, ForceMode.Force);

        // 감속 처리 (입력이 없을 때 점차적으로 속도 감소)
        if (moveInput.magnitude == 0)
        {
            // 속도 감소 (입력 없을 때)
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * GlobalSettings.Instance.BoosterDeceleration);  // 천천히 멈추게 함
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
