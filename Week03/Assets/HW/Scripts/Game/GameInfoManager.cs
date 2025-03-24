using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    public static GameInfoManager Instance => _instance;
    static GameInfoManager _instance;

    public static List<int> ObjectiveByStage => _objectiveByStage;
    static List<int> _objectiveByStage;

    private void Awake()
    {
        //Singleton 초기화.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        Initialize();
        Invoke("TriggerAction", 0.1f);
    }

    private void Start()
    {

    }

    [Header("GameInfos")]
    public int HP => _HP; int _HP; //reamining HP.
    public int Mineral => _mineral; int _mineral; //score.
    public float Resource => _resource; float _resource; //resource.
    public float ResourceRecover => _resourceRecover; float _resourceRecover;

    public int CurrentStage => _currentStage; [SerializeField]int _currentStage = 1;
    public int LastStage => _lastStage; int _lastStage;

    [Header("Actions")]
    public Action<int> HPUpdateAction;
    public Action<int, int> MineralUpdateAction;
    public Action<float> ResourceUpdateAction;

    [Header("Resource Usages")]
    public float DashResourceUsage => _dashResourceUsage; float _dashResourceUsage;
    public float AirDashResourceUsage => _airDashResourceUsage; float _airDashResourceUsage;
    public float AirJumpResourceUsagePerSec => _airJumpResourceUsage; float _airJumpResourceUsage;
    void Initialize()
    {
        _HP = 100; 
        _mineral = 0; 
        _resource = 100f;
        _dashResourceUsage = 30;
        _airDashResourceUsage = 30;
        _resourceRecover = 30;
        _airJumpResourceUsage = 30f;

        //_currentStage = 1;
        _objectiveByStage = new List<int> {0, 50, 100, 150, 200, 300};
    }

    void TriggerAction()
    {
        HPUpdateAction?.Invoke(_HP);
        MineralUpdateAction?.Invoke(_mineral, _objectiveByStage[_currentStage]);
        ResourceUpdateAction?.Invoke(_resource);
    }

    public void SetHP(int newValue)
    {
        _HP = newValue;
        HPUpdateAction?.Invoke(_HP);
    }

    public void UpdateHP(int updateValue)
    {
        _HP += updateValue;
        HPUpdateAction?.Invoke(_HP);
    }

    public void SetMineral(int newValue)
    {
        _mineral = newValue;
        MineralUpdateAction?.Invoke(_mineral, _objectiveByStage[_currentStage]);
    }

    public void UpdateMineral(int updateValue)
    {
        _mineral += updateValue;
        MineralUpdateAction?.Invoke(_mineral, _objectiveByStage[_currentStage]);
    }

    public void SetResource(float newValue)
    {
        _resource = newValue;

        if (_resource > 100) _resource = 100;
        else if (_resource < -30) _resource = -30;
            ResourceUpdateAction?.Invoke(_resource);
    }

    public void UpdateResource(float updateValue)
    {
        _resource += updateValue;
        if (_resource > 100) _resource = 100;
        else if (_resource < -30) _resource = -30;
        ResourceUpdateAction?.Invoke(_resource);
    }
}
