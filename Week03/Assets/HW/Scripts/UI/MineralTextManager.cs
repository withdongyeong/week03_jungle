using System.Collections;
using TMPro;
using UnityEngine;

public class MineralTextManager : MonoBehaviour
{
    TextMeshProUGUI mineralText;

    float moveDuration = 0.3f;

    private void Awake()
    {
        mineralText = GetComponent<TextMeshProUGUI>();

        
    }

    private void Start()
    {
        GameInfoManager.Instance.MineralUpdateAction += SetValue;

        mineralText.text = "Mineral\n0 / " + GameInfoManager.ObjectiveByStage[GameInfoManager.Instance.CurrentStage];
    }

    void SetValue(int newValue, int max)
    {
        StopCoroutine("UpdateMineralTextCoroutine");
        StartCoroutine(UpdateMineralTextCoroutine(newValue, max));
    }

    private IEnumerator UpdateMineralTextCoroutine(int newValue, int max)
    {
        float elapsedTime = 0f;
        float startValue = GetCurrentMineralValue(); // 현재 값 파싱해서 가져오기
        Debug.Log(startValue + " " + newValue + " ");
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            
            
            // Lerp로 현재 값에서 목표 값까지 보간
            float currentValue = Mathf.Lerp(startValue, newValue, t);
            mineralText.text = $"Mineral\n{Mathf.RoundToInt(currentValue)} / " + max; ;

            yield return null; // 다음 프레임까지 대기
        }

        // 정확히 목표 값에 도달하도록 마지막 설정
        mineralText.text = $"Mineral\n{newValue} / " + max;
    }

    private float GetCurrentMineralValue()
    {
        if (string.IsNullOrEmpty(mineralText.text))
            return 0f;

        // "Mineral : 50 / 100" 형식에서 숫자 부분 추출
        string[] parts = mineralText.text.Split(' ');
        parts[0] = parts[0].Replace("Mineral\n", "");
        
        if (parts.Length >= 3 && float.TryParse(parts[0], out float value))
        {
            return value;
            
        }
        return 0f; // 파싱 실패 시 기본값 0
    }
}
