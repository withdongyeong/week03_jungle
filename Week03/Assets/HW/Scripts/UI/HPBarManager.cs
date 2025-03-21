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
        float elapsedTime = 0f;
        float startValue = HPSlider.value; // ���� �� ����

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration; // 0���� 1�� ����� ���

            // Lerp�� ����� ���� ������ ��ǥ ������ �ε巴�� �̵�
            HPSlider.value = Mathf.Lerp(startValue, newValue, t);

            yield return null; // ���� �����ӱ��� ���
        }

        // ��Ȯ�� ��ǥ ���� �����ϵ��� �������� ���� ����
        HPSlider.value = newValue;
    }
}
