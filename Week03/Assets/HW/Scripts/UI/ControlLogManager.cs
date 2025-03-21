using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlLogManager : MonoBehaviour
{
    public static ControlLogManager Instance => _instance;
    private static ControlLogManager _instance;

    private TextMeshProUGUI controlLogText;
    private float loadPanelDuration = 0.15f;

    private void Awake()
    {
        _instance = this;
        controlLogText = GetComponent<TextMeshProUGUI>();
    }

    // List로 받은 매핑을 인덱스로 표시 (키보드 - 게임패드 - 텍스트 순서)
    public void SetControlLogText(List<(int keyboardSpriteIndex, int controllerSpriteIndex, string actionText)> controlMappings)
    {
        string displayText = BuildControlText(controlMappings);
        StopCoroutine(LoadControlLogTextPanel());
        controlLogText.text = displayText;
        StartCoroutine(LoadControlLogTextPanel());
    }

    private string BuildControlText(List<(int keyboardSpriteIndex, int controllerSpriteIndex, string actionText)> mappings)
    {
        string result = "";
        foreach (var (keyboardSpriteIndex, controllerSpriteIndex, actionText) in mappings)
        {
            // 키보드 스프라이트 - 게임패드 스프라이트 - 텍스트 순으로 표시 (인덱스 사용)
            result += $"<sprite={keyboardSpriteIndex}>    <sprite={controllerSpriteIndex}> {actionText}\n";
        }
        return result.TrimEnd('\n'); // 마지막 줄바꿈 제거
    }

    private IEnumerator LoadControlLogTextPanel()
    {
        float startPosX = -205f;
        float endPosX = 0f;
        Vector3 startPos = new Vector3(startPosX, controlLogText.rectTransform.anchoredPosition.y, 0);
        Vector3 endPos = new Vector3(endPosX, controlLogText.rectTransform.anchoredPosition.y, 0);

        controlLogText.rectTransform.anchoredPosition = startPos;
        controlLogText.alpha = 0.3f;

        float elapsedTime = 0f;
        while (elapsedTime < loadPanelDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / loadPanelDuration;
            controlLogText.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            controlLogText.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        controlLogText.rectTransform.anchoredPosition = endPos;
        controlLogText.alpha = 1f;
    }
}