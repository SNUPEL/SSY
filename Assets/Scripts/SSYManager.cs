using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SSYManager : MonoBehaviour
{

    private static SSYManager mInstance;
    private static VisualElement mElement;

    private int mTimestamp = 0;
    private float mSpent = 0f;
    private float mStep = 0.1f;

    private const string mUrlReshuffle = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\reshuffle.csv";
    private const string mUrlRetrieval = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\retrieval.csv";
    private const string mUrlResult = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\result.csv";

    public static SSYManager GetInstance()
    {
        if (mInstance == null)
        {
            mInstance = FindObjectOfType<SSYManager>();
            if (mInstance == null )
            {
                GameObject _gameObject = new GameObject(typeof(SSYManager).Name);
                mInstance = _gameObject.AddComponent<SSYManager>();
            }
        }
        return mInstance;
    }

    public float Spent
    {
        get { return mSpent; }
        set { mSpent = value; }
    }

    private void Awake()
    {
        mElement = GetComponent<UIDocument>().rootVisualElement;
    }

    // Start is called before the first frame update
    void Start()
    {
        StockLayout.GetInstance().InitializeLayout();
        SteelManager.GetInstance().InitializePiles(mUrlReshuffle);
        SteelManager.GetInstance().InitializePiles(mUrlRetrieval);
        CraneManager.GetInstance().InitializeCrane();
        DiscreteEventManager.GetInstance().InitializeEvents(mUrlResult);

        InitializeSimulation();
    }

    /// <summary>
    /// 시뮬레이션 환경을 초기화합니다.
    /// </summary>
    private void InitializeSimulation()
    {
        foreach (var crane in CraneManager.GetInstance().Cranes)
        {
            foreach (var discreteEvent in DiscreteEventManager.GetInstance().Events)
            {
                if (discreteEvent.Value.Timestamp == 0 && discreteEvent.Value.Crane.name == crane.name)
                {
                    crane.GetComponent<CraneController>().Locate(discreteEvent.Value.Location);
                    crane.GetComponent<CraneController>().SetPreviousEvent(discreteEvent);
                    continue;
                }
                if (discreteEvent.Value.Crane.Name == crane.name)
                {
                    crane.GetComponent<CraneController>().SetCurrentEvent(discreteEvent);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Game 모드에서 오류 내용 등을 보여줍니다.
    /// </summary>
    /// <param name="message"></param>
    public static void Log(string message)
    {
        Label _label = mElement.Q<Label>("labelLog");
        _label.text = message;
        Debug.Log(message);
    }

    // Update is called once per frame
    void Update()
    {
        if (DiscreteEventManager.GetInstance().Events.Count == 0)
            return;
        if (Spent >= DiscreteEventManager.GetInstance().Events.Last().Key)
            InitializeSimulation();

        Spent += mStep;
        CraneManager.GetInstance().Move(Spent, mStep);
        if (DiscreteEventManager.GetInstance().Events.ContainsKey((int)Spent))
            if (DiscreteEventManager.GetInstance().Events[(int)Spent].State == Mode.RELEASE)
                StockLayout.GetInstance().Release(DiscreteEventManager.GetInstance().Events[(int)Spent].Location);
    }

    
}
