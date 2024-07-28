using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SteelManager : MonoBehaviour
{
    private static SteelManager mInstance;
    private StockLayout mStockLayout;
    private List<GameObject> mSteels;
    private bool isFirstLine = true;
    private const int mIndexPileNo = 0;
    private const int mIndexPileSeq = 1;
    private const int mIndexMarkNo = 2;
    private const int mIndexUnitW = 3;
    private const int mIndexToPile = 4;

    // 알고리즘 A에 대한 결과 시나리오
    //private const string mUrlReshuffle_withA = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\reshuffle_A.csv";
    //private const string mUrlRetrieval_withA = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\retrieval_A.csv";

    // 알고리즘 B에 대한 결과 시나리오

    public GameObject mPrefabSteel;
    private GameObject mParentSteel;

    public GameObject ParentSteel
    {
        get { return mParentSteel; }
        set { mParentSteel = value; }
    }

    public void SetStockLayout (StockLayout stockLayout)
    {
        this.mStockLayout = stockLayout;
        ParentSteel.transform.SetParent(stockLayout.transform.parent);
    }

    public List<GameObject> Steels
    {
        get
        {
            if (mSteels == null)
                mSteels = new List<GameObject>();
            return mSteels;
        }
        set { }
    }

    public void Awake()
    {
        ParentSteel = new GameObject("Steels");
        mPrefabSteel = Resources.Load<GameObject>("Prefabs/steel");
    }

    public void Initialize(string[] urlList)
    {
        foreach (var url in urlList)
            InitializePiles(url);
        // 알고리즘 B 가시화에 사용할 강재 적치
        //InitializePiles(mUrlReshuffle_withB);
        //InitializePiles(mUrlRetrieval_withB);
    }

    /// <summary>
    /// 파일 데이터를 읽어들여서 강재를 적치함
    /// </summary>
    /// <param name="filePath"></param>
    private void InitializePiles(string filePath)
    {
        if (!File.Exists(filePath))
        {
            SSYManager.Log(string.Format("{0} 파일이 존재하지 않습니다. \nSteel 생성에 실패하였습니다.", filePath));
            return;
        }

        StreamReader _streamReader = new StreamReader(filePath);

        while(!_streamReader.EndOfStream)
        {
            string _line = _streamReader.ReadLine();
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }
            string[] _data = _line.Split(',');
            SteelBuilder _steelBuilder = new SteelBuilder();
            GameObject _steel = Instantiate(mPrefabSteel, mStockLayout.getExactSteelLocation(_data[mIndexPileNo]), Quaternion.identity);
            _steel.name = string.Format(_data[mIndexMarkNo]);
            _steel.GetComponent<Steel>().Initialize(_steelBuilder.setPileNo(_data[mIndexPileNo]).setPileSequence(_data[mIndexPileSeq]).setMarkNo(_data[mIndexMarkNo]).setUnitW(_data[mIndexUnitW]).setToPile(_data[mIndexToPile]).Build());
            mStockLayout.InitializeSteels(_data[mIndexPileNo], _steel);
            _steel.transform.parent = mParentSteel.transform;
            Steels.Add(_steel);
        }
        isFirstLine = true;
    }
}
