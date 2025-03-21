using System;
using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    public static GameInfoManager Instance => _instance;
    static GameInfoManager _instance;

    private void Awake()
    {
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

    [Header("Actions")]
    public Action<int> HPUpdateAction;
    public Action<int> MineralUpdateAction;
    public Action<float> ResourceUpdateAction;

    [Header("Resource Usages")]
    public float DashResourceUsage => _dashResourceUsage; float _dashResourceUsage;
    public float AirDashResourceUsage => _airDashResourceUsage; float _airDashResourceUsage;

    void Initialize()
    {
        _HP = 100; 
        _mineral = 0; 
        _resource = 100f;
        _dashResourceUsage = 30;
        _airDashResourceUsage = 30;
        _resourceRecover = 30;
    }

    void TriggerAction()
    {
        HPUpdateAction?.Invoke(_HP);
        MineralUpdateAction?.Invoke(_mineral);
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
        MineralUpdateAction?.Invoke(_mineral);
    }

    public void UpdateMineral(int updateValue)
    {
        _mineral += updateValue;
        MineralUpdateAction?.Invoke(_mineral);
    }

    public void SetResource(int newValue)
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
