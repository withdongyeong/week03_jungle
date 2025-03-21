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
        float startValue = resourceSlider.value; // ���� �� ����

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration; // 0���� 1�� ����� ���

            // Lerp�� ����� ���� ������ ��ǥ ������ �ε巴�� �̵�
            resourceSlider.value = Mathf.Lerp(startValue, newValue, t);

            yield return null; // ���� �����ӱ��� ���
        }

        // ��Ȯ�� ��ǥ ���� �����ϵ��� �������� ���� ����
        resourceSlider.value = newValue;
    }


}
