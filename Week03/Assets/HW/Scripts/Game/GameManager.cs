using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // SceneManager를 사용하기 위해 추가

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => _instance;
    static GameManager _instance;

    InputSystem_Actions actions;

    GameInfoManager gameInfoManager;
    //[SerializeField] GameObject cam;

    [SerializeField] string successCanvasName;

    private void Awake()
    {
        //Singleton 초기화.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1;

        gameInfoManager = GameInfoManager.Instance;
        gameInfoManager.MineralUpdateAction += CheckSuccess;
        gameInfoManager.HPUpdateAction += CheckFail;

        //cam.SetActive(true);
    }

    private void CheckFail(int currentHP)
    {
        if(currentHP < 0)
        {

            PlayerMoveManager.Instance.RestartCurrentScene();
        }

    }

    private void CheckSuccess(int currentMineral, int max)
    {
        if(currentMineral >= max)
        {
            Time.timeScale = 0;

            Instantiate((GameObject)Resources.Load("HW/UI/" + successCanvasName));
        }
    }
}