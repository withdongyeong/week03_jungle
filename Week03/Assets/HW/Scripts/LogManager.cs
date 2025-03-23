using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class LineData
{
    [SerializeField] public string code;
    [SerializeField] public string talker;
    [SerializeField] public string line;
    [SerializeField] public string side; //a -> ally, e -> enemy.
    [SerializeField] public float talkingTime;
}

[System.Serializable]
public class LineDataWrapper
{
    public List<LineData> lines; // JSON 배열을 매핑
}


public class LogManager : MonoBehaviour
{
    public static LogManager Instance => _instance;
    static LogManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    GameInfoManager gameInfoManager;
    TextAsset jsonFile;

    private List<LineData> lineDataList; // 여러 LineData를 저장


    LineData lineData;
    bool isTalking = false;

    [SerializeField] List<string> queueInitialize;
    [SerializeField] Queue<string> q;

    TextMeshProUGUI talkerText; //화자의 tmp
    TextMeshProUGUI lineText; //대사의 tmp

    private void Start()
    {
        q = new Queue<string>(queueInitialize); //queue 초기화.

        gameInfoManager = GameInfoManager.Instance;

        LoadLineDataFromJson(); //json 읽기.

        gameInfoManager.HPUpdateAction += HpUpdateLineAction;
        gameInfoManager.MineralUpdateAction += MineralUpdateLineAction;
        gameInfoManager.ResourceUpdateAction += ResourceUpdateLineAction;


        talkerText = GameObject.Find("Talker").GetComponent<TextMeshProUGUI>();
        lineText = GameObject.Find("Line").GetComponent<TextMeshProUGUI>();
        talkerText.color = new Color(0, 0, 0, 0); //transparent.
        lineText.color = new Color(0, 0, 0, 0); //transparent.
    }

    private void Update()
    {
        if(!isTalking)
        {
            if(q.Count > 0) //대사 큐가 1개 이상
            {
                string lineCode = q.Dequeue();
                InvokeLine(lineCode);
            }
        }
    }

    public void AddLineToQueue(int code)
    {

    }

    public void InvokeLine(string code)
    {
        LineData newLineData = GetLineDataByCode(code);

        isTalking = true; //now talking.

        //Set Color.
        if(newLineData.side == "e")
        {
            talkerText.color = Color.red; //transparent?
        }
        if (newLineData.side == "a")
        {
            talkerText.color = Color.blue; //transparent?
        }

        lineText.color = Color.white;

        //Set Text
        talkerText.text = "< " + newLineData.talker + " >";
        lineText.text = newLineData.line;


        float duration = newLineData.talkingTime;
        Invoke("LineEnd", duration + 0.2f);
    }

    private void LineEnd()
    {
        //Set Color Transparent.
        talkerText.color = new Color(0, 0, 0, 0); //transparent.
        lineText.color = new Color(0, 0, 0, 0); //transparent.

        isTalking = false;
    }


    [ContextMenu("From Json Data")]
    void LoadLineDataFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("lineData");

        if (jsonFile != null)
        {
            Debug.Log("Loaded JSON content: " + jsonFile.text);
            LineDataWrapper wrapper = JsonConvert.DeserializeObject<LineDataWrapper>(jsonFile.text);
            if (wrapper != null && wrapper.lines != null)
            {
                lineDataList = wrapper.lines;
                foreach (var data in lineDataList)
                {
                    Debug.Log($"Loaded: {data.code}, {data.talker}, {data.side}, {data.talkingTime}");
                }
            }
            else
            {
                Debug.LogError("Newtonsoft failed: 'lines' is null.");
            }
        }
        else
        {
            Debug.LogError("JSON file 'lineData' not found in Resources folder.");
        }
    }
    // code로 데이터 조회 (옵션)
    public LineData GetLineDataByCode(string code)
    {
        return lineDataList?.Find(line => line.code == code);
    }

    int hp_last;
    float ratio_last;
    int resource_last;

    void HpUpdateLineAction(int hp)
    {
        if(hp_last >= 75 && hp < 75)
        {
            q.Enqueue("hp1_" + gameInfoManager.CurrentStage);
        }
        else if (hp_last >= 50 && hp < 50)
        {
            q.Enqueue("hp2_" + gameInfoManager.CurrentStage);
        }
        else if (hp_last >= 25 && hp < 25)
        {
            q.Enqueue("hp3_" + gameInfoManager.CurrentStage);
        }

        hp_last = hp;
    }

    void MineralUpdateLineAction(int mineral, int max)
    {
        float ratio = mineral / max; //현재 획득 점수 비율.
        
        if (ratio_last <= 25  && ratio > 25)
        {
            q.Enqueue("mineral1_" + gameInfoManager.CurrentStage);
        }
        else if (ratio_last <= 50 && ratio > 50)
        {
            q.Enqueue("mineral2_" + gameInfoManager.CurrentStage);
        }
        else if (ratio_last <= 75 && ratio > 75)
        {
            q.Enqueue("mineral3_" + gameInfoManager.CurrentStage);
        }

        ratio_last = ratio;
    }

    void ResourceUpdateLineAction(float resource)
    {

    }


}
