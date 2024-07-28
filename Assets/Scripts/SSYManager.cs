using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SSYManager : MonoBehaviour
{
    private StockLayout mStockLayout;
    private SteelManager mSteelManager;
    private CraneManager mCraneManager;
    private DiscreteEventManager mDiscreteEventManager;

    private int mTimestamp = 0;
    private float mSpent = 0f;
    private const float mStep = 0.1f;

    public Camera mCamera;
    public TextAsset mUrlResult;
    public TextAsset mUrlReshuffle;
    public TextAsset mUrlRetrieval;
    public Vector3 mPivot;

    private float mCraneTravelTimeWithoutSteel = 0f;
    private float mAvoidingWaitingTime = 0f;
    private float mWaitingTime = 0f;

    private float mWidth = 0.30f;
    private float mHeight = 0.02f;

    public StockLayout StockLayout
    {
        get
        {
            if (mStockLayout.GetComponent<StockLayout>() == null)
                mStockLayout.AddComponent<StockLayout>();
            return mStockLayout.GetComponent<StockLayout>();
        }
    }

    public DiscreteEventManager DiscreteEventManager
    {
        get
        {
            if (mDiscreteEventManager.GetComponent<DiscreteEventManager>() == null)
                mDiscreteEventManager.AddComponent<DiscreteEventManager>();
            return mDiscreteEventManager.GetComponent<DiscreteEventManager>();
        }
    }

    public float Spent
    {
        get { return mSpent; }
        set { mSpent = value; }
    }

    public float TravelingTimeWithoutSteel
    {
        get { return mCraneTravelTimeWithoutSteel; }
        set { mCraneTravelTimeWithoutSteel = value; }
    }

    public float AvoidingWaitingTime
    {
        get { return mAvoidingWaitingTime; }
        set { mAvoidingWaitingTime = value; }
    }

    public float WaitingTime
    {
        get { return mWaitingTime; }
        set { mWaitingTime = value; }
    }

    private void Start()
    {
        Initialize(AssetDatabase.GetAssetPath(mUrlReshuffle), AssetDatabase.GetAssetPath(mUrlRetrieval), AssetDatabase.GetAssetPath(mUrlResult), mPivot);
    }


    public void Initialize(string urlReshuffle, string urlRetrieval, string urlResult, Vector3 pivot)
    {
        mStockLayout = new GameObject("StockLayout").AddComponent<StockLayout>().GetComponent<StockLayout>();
        mStockLayout.transform.SetParent(this.transform);
        mStockLayout.GetComponent<StockLayout>().Initialize(pivot);

        mSteelManager = new GameObject("SteelManager").AddComponent<SteelManager>().GetComponent<SteelManager>();
        mSteelManager.transform.SetParent(this.transform);
        mSteelManager.SetStockLayout(mStockLayout);
        mSteelManager.Initialize(new string[] { urlReshuffle, urlRetrieval });

        mCraneManager = new GameObject("CraneManager").AddComponent<CraneManager>().GetComponent<CraneManager>();
        mDiscreteEventManager = new GameObject("DiscreteEventManager").AddComponent<DiscreteEventManager>().GetComponent<DiscreteEventManager>();
        mCraneManager.transform.SetParent(this.transform);
        mCraneManager.SetDiscreteEventManager(mDiscreteEventManager);
        mCraneManager.Initialize();

        mDiscreteEventManager.transform.SetParent(this.transform);
        mDiscreteEventManager.Initialize(urlResult);

        InitializeSimulation();
    }


    /// <summary>
    /// 시뮬레이션 환경을 초기화합니다.
    /// </summary>
    private void InitializeSimulation()
    {
        foreach (var crane in mCraneManager.Cranes)
        {
            foreach (var discreteEvent in mDiscreteEventManager.Events)
            {
                if (discreteEvent.Value.Timestamp == 0 && discreteEvent.Value.Crane.name == crane.name)
                {
                    crane.GetComponent<Crane>().Locate(discreteEvent.Value.Location);
                    crane.GetComponent<Crane>().SetPreviousEvent(discreteEvent);
                    continue;
                }
                if (discreteEvent.Value.Crane.Name == crane.name)
                {
                    crane.GetComponent<Crane>().SetCurrentEvent(discreteEvent);
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
        Label _label = new Label();
        _label.text = message;
        Debug.Log(message);
    }

    // Update is called once per frame
    void Update()
    {
        if (mDiscreteEventManager == null) return;
        if (mDiscreteEventManager.Events.Count == 0)
            return;
        if (Spent >= mDiscreteEventManager.Events.Last().Key)
            InitializeSimulation();

        Spent += mStep;
        mCraneManager.Move(Spent, mStep);
        if (mDiscreteEventManager.Events.ContainsKey((int)Spent))
            if (mDiscreteEventManager.Events[(int)Spent].State == Mode.RELEASE)
                mStockLayout.GetComponent<StockLayout>().Release(mDiscreteEventManager.Events[(int)Spent].Location);
    }

    public Crane GetCrane(string crane)
    {
        return mCraneManager.GetCrane(crane);
    }

    public GameObject FindSteel(string steel)
    {
        return mSteelManager.Steels.Find(x => x.name == steel);
    }

    public GameObject GetParentSteel()
    {
        return mSteelManager.ParentSteel;
    }

    private void OnGUI()
    {
        GUIStyle _style = new GUIStyle();
        _style.fontSize = 15;
        _style.normal.textColor = Color.black;
        string _method = mUrlResult.name;


        Rect _rect = new Rect(Screen.width * (mCamera.rect.x + mWidth), Screen.height * (mCamera.rect.y + mHeight), mWidth, mHeight);

        string _data = String.Format("Algorithm: {0}\nTraveling Without Steel: {1}\nAvoiding Waiting Time: {2}\nWaiting Time: {3}", 
            _method, 
            mCraneTravelTimeWithoutSteel.ToString("F0"), 
            mAvoidingWaitingTime.ToString("F0"), 
            mWaitingTime.ToString("F0"));
        GUI.Label(_rect, _data, _style);
    }
}
