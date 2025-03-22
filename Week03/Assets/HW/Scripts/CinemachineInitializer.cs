using Unity.Cinemachine;
using UnityEngine;

public class CinemachineInitializer : MonoBehaviour
{
    private CinemachineInputAxisController inputAxisController;

    private void Awake()
    {
        // 컴포넌트 가져오기
        inputAxisController = GetComponent<CinemachineInputAxisController>();
        if (inputAxisController == null)
        {
            Debug.LogError("CinemachineInputAxisController not found on " + gameObject.name);
            return;
        }

        // 초기 비활성화
        inputAxisController.enabled = false;
    }

    private void Start()
    {
        // 플레이 시작 후 일정 시간 뒤 활성화 (예: 1초)
        Invoke(nameof(EnableCinemachineInput), 1f);
    }

    private void EnableCinemachineInput()
    {
        if (inputAxisController != null)
        {
            inputAxisController.enabled = true;
            Debug.Log("CinemachineInputAxisController enabled");
        }
    }

    private void OnDestroy()
    {
        // 정리: 비활성화
        if (inputAxisController != null)
        {
            inputAxisController.enabled = false;
        }
    }
}