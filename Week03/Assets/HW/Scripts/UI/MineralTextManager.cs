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

        mineralText.text = "Mineral : 0 / 100";
    }

    private void Start()
    {
        GameInfoManager.Instance.MineralUpdateAction += SetValue;
    }

    void SetValue(int newValue)
    {
        StopCoroutine("UpdateMineralTextCoroutine");
        StartCoroutine(UpdateMineralTextCoroutine(newValue));
    }

    private IEnumerator UpdateMineralTextCoroutine(int newValue)
    {
        float elapsedTime = 0f;
        float startValue = GetCurrentMineralValue(); // ���� �� �Ľ��ؼ� ��������

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            // Lerp�� ���� ������ ��ǥ ������ ����
            float currentValue = Mathf.Lerp(startValue, newValue, t);
            mineralText.text = $"Mineral : {Mathf.RoundToInt(currentValue)} / 100";

            yield return null; // ���� �����ӱ��� ���
        }

        // ��Ȯ�� ��ǥ ���� �����ϵ��� ������ ����
        mineralText.text = $"Mineral : {newValue} / 100";
    }

    private float GetCurrentMineralValue()
    {
        if (string.IsNullOrEmpty(mineralText.text))
            return 0f;

        // "Mineral : 50 / 100" ���Ŀ��� ���� �κ� ����
        string[] parts = mineralText.text.Split(' ');
        if (parts.Length >= 3 && float.TryParse(parts[2], out float value))
        {
            return value;
        }
        return 0f; // �Ľ� ���� �� �⺻�� 0
    }
}
