using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class NextStageButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttonText;
    private InputSystem_Actions actions;
    private bool wasPlayerEnabled; // Player 액션 맵의 원래 상태 저장

    private void Awake()
    {
        // Button 컴포넌트 가져오기
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component not found on " + gameObject.name);
            return;
        }

        // 하위 Text 컴포넌트 가져오기
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
            Debug.LogError("Text component not found in children of " + gameObject.name);
            return;
        }

        // 초기 텍스트 설정
        buttonText.text = "> On To The Next Bout";

        // InputSystem_Actions 초기화
        actions = HW_PlayerStateController.Instance.GetInputActions();
        if (actions == null)
        {
            Debug.LogError("InputSystem_Actions not found!");
            return;
        }

        // Player 액션 맵의 원래 상태 저장
        wasPlayerEnabled = actions.Player.enabled;

        // 입력 모드 설정
        actions.Player.Disable(); // Player 입력 비활성화
        actions.UI.Enable();      // UI 입력 활성화

        // Submit 이벤트 연결
        actions.UI.Submit.started += OnSubmitStarted;
        actions.UI.Submit.performed += OnSubmitPerformed;

        // 처음부터 선택 상태로 설정
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    private void OnSubmitStarted(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            Debug.Log("Submit started - Button pressed");
            SetButtonText(">On To The Next Bout");
        }
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            Debug.Log("Submit performed - Next stage triggered");
            OnButtonClick();
        }
    }

    public void SetButtonText(string newText)
    {
        if (buttonText != null)
        {
            buttonText.text = newText;
        }
    }

    private void OnButtonClick()
    {
        int nextStage = GameInfoManager.Instance.CurrentStage + 1;
        string nextSceneName = "The Last Prove " + nextStage;

        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Scene not found: " + nextSceneName);
            SetButtonText("> No More Stages");
        }
    }

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        // UI 이벤트 해제
        if (actions != null)
        {
            actions.UI.Submit.started -= OnSubmitStarted;
            actions.UI.Submit.performed -= OnSubmitPerformed;

            // 입력 상태 복원
            actions.UI.Disable();
            if (wasPlayerEnabled)
            {
                actions.Player.Enable(); // Player 원래 상태로 복구
            }
        }

        // 버튼 이벤트 정리
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}