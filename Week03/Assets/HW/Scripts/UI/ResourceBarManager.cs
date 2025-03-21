using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarManager : MonoBehaviour
{
    //public static ResourceBarManager Instance => _instance;
    //static ResourceBarManager _instance;

    //Slider Reference.
    Slider resourceSlider;

    //UI Design Reference.
    float moveDuration = 0.3f;

    private void Awake()
    {
        //_instance = this;

        resourceSlider = GetComponent<Slider>();
        resourceSlider.minValue = 0;
        resourceSlider.maxValue = 100;
        resourceSlider.wholeNumbers = false;
        resourceSlider.value = 0;



    }

    private void Start()
    {
        GameInfoManager.Instance.ResourceUpdateAction += SetValue;
    }

    void SetValue(int newValue)
    {
        StopCoroutine("UpdateResourceSliderCoroutine");
        StartCoroutine(UpdateResourceSliderCoroutine(newValue));
    }

    private IEnumerator UpdateResourceSliderCoroutine(int newValue)
    {
        float elapsedTime = 0f;
        float startValue = resourceSlider.value; // 시작 값 저장

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration; // 0에서 1로 진행률 계산

            // Lerp를 사용해 현재 값에서 목표 값으로 부드럽게 이동
            resourceSlider.value = Mathf.Lerp(startValue, newValue, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 정확히 목표 값에 도달하도록 마지막에 강제 설정
        resourceSlider.value = newValue;
    }


}
