using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBarManager : MonoBehaviour
{
    //public static HPBarManager Instance => _instance;
    //static HPBarManager _instance;

    private void Awake()
    {
        //_instance = this;

        HPSlider = GetComponent<Slider>();
        HPSlider.minValue = 0;
        HPSlider.maxValue = 100;
        HPSlider.wholeNumbers = false;
        HPSlider.value = 0;



    }

    private void OnDestroy()
    {
        StopCoroutine("UpdateHPSliderCoroutine");
    }

    private void Start()
    {
        GameInfoManager.Instance.HPUpdateAction += SetValue;
    }

    void SetValue(int newValue)
    {
        StopCoroutine("UpdateHPSliderCoroutine");
        StartCoroutine(UpdateHPSliderCoroutine(newValue));
    }

    //Slider Reference.
    Slider HPSlider;

    //UI Design Reference.
    float moveDuration = 0.3f;

    private IEnumerator UpdateHPSliderCoroutine(int newValue)
    {
        try
        {
            float elapsedTime = 0f;
            float startValue = HPSlider.value; // 시작 값 저장

            while (elapsedTime < moveDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / moveDuration; // 0에서 1로 진행률 계산

                // Lerp를 사용해 현재 값에서 목표 값으로 부드럽게 이동
                HPSlider.value = Mathf.Lerp(startValue, newValue, t);

                yield return null; // 다음 프레임까지 대기
            }

            //// 정확히 목표 값에 도달하도록 마지막에 강제 설정
            //HPSlider.value = newValue;
        }
        finally
        {
            // 정확히 목표 값에 도달하도록 마지막에 강제 설정
            HPSlider.value = newValue;
        }


       
    }
}
