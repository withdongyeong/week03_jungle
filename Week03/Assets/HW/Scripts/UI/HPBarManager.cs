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
        HPSlider.value = newValue;
    }

    //Slider Reference.
    Slider HPSlider;

  
}
