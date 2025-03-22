using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarManager : MonoBehaviour
{
    Slider resourceSlider;
    Image fillRectImage;
    Image backgroundImage; // 슬라이더의 배경 이미지

    float moveDuration = 0.05f;
    float blinkInterval = 0.5f; // 점멸 간격 (0.5초마다 깜빡임)
    private Coroutine blinkCoroutine; // 점멸 코루틴 참조

    private void Awake()
    {
        resourceSlider = GetComponent<Slider>();
        resourceSlider.minValue = 0;
        resourceSlider.maxValue = 100;
        resourceSlider.wholeNumbers = false;
        resourceSlider.value = 0;

        fillRectImage = resourceSlider.fillRect.GetComponent<Image>();
        backgroundImage = resourceSlider.transform.Find("Background").GetComponent<Image>(); // 배경 이미지 참조
    }

    private void Start()
    {
        GameInfoManager.Instance.ResourceUpdateAction += SetValue;
    }

    void SetValue(float newValue)
    {
        StopCoroutine("UpdateResourceSliderCoroutine");
        StartCoroutine(UpdateResourceSliderCoroutine(newValue));
    }

    private IEnumerator UpdateResourceSliderCoroutine(float newValue)
    {
        float elapsedTime = 0f;
        float startValue = resourceSlider.value;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            resourceSlider.value = Mathf.Lerp(startValue, newValue, t);
            UpdateSlider(newValue);

            yield return null;
        }

        resourceSlider.value = newValue;
        UpdateSlider(newValue); // 최종 값 반영

        // 0 이하일 때 점멸 시작/중지
        if (newValue <= 0)
        {
            if (blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkBackground());
            }
        }
        else
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
                backgroundImage.color = Color.gray; // 기본 색상으로 복구
            }
        }
    }

    private void UpdateSlider(float newValue)
    {
        if (newValue < 0.001f)
        {
            Color currentColor = fillRectImage.color;
            currentColor.a = 0;
            fillRectImage.color = currentColor;
        }
        else
        {
            Color currentColor = fillRectImage.color;
            currentColor.a = 1;
            fillRectImage.color = currentColor;
        }
    }

    // 배경을 빨갛게 점멸시키는 코루틴
    private IEnumerator BlinkBackground()
    {
        while (true)
        {
            backgroundImage.color = Color.red; // 빨간색
            yield return new WaitForSeconds(blinkInterval / 2); // 절반 시간 대기
            backgroundImage.color = Color.gray; // 원래 색상 (흰색)
            yield return new WaitForSeconds(blinkInterval / 2); // 나머지 절반 대기
        }
    }
}