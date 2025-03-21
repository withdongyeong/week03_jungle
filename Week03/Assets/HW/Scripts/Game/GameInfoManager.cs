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
    public int Resource => _resource; int _resource; //resource.

    [Header("Actions")]
    public Action<int> HPUpdateAction;
    public Action<int> MineralUpdateAction;
    public Action<int> ResourceUpdateAction;

    void Initialize()
    {
        _HP = 100; 
        _mineral = 0; 
        _resource = 100; 
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
        ResourceUpdateAction?.Invoke(_resource);
    }

    public void UpdateResource(int updateValue)
    {
        _resource += updateValue;
        ResourceUpdateAction?.Invoke(_resource);
    }
}
