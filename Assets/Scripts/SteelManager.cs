using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SteelManager : MonoBehaviour
{
    private static SteelManager mInstance;
    private List<GameObject> mSteels;
    private bool isFirstLine = true;
    private const int mIndexPileNo = 0;
    private const int mIndexPileSeq = 1;
    private const int mIndexMarkNo = 2;
    private const int mIndexUnitW = 3;
    private const int mIndexToPile = 4;

    // �˰��� A�� ���� ��� �ó�����
    private const string mUrlReshuffle_withA = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\reshuffle_A.csv";
    private const string mUrlRetrieval_withA = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\retrieval_A.csv";

    // �˰��� B�� ���� ��� �ó�����
    private const string mUrlReshuffle_withB = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\reshuffle_B.csv";
    private const string mUrlRetrieval_withB = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\retrieval_B.csv";

    public GameObject mPrefabSteel;
    private GameObject mParentSteel;

    public static SteelManager GetInstance()
    {
        if (mInstance == null)
        {
            mInstance = FindObjectOfType<SteelManager>();
            if (mInstance == null)
            {
                GameObject _gameObject = new GameObject(typeof(SteelManager).Name);
                mInstance = _gameObject.AddComponent<SteelManager>();
            }
        }
        return mInstance;
    }

    public GameObject ParentSteel
    {
        get { return mParentSteel; }
        set { mParentSteel = value; }
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
    }

    public void Initialize()
    {
        // �˰��� A ����ȭ�� ����� ���� ��ġ
        InitializePiles(mUrlReshuffle_withA);
        InitializePiles(mUrlRetrieval_withA);

        // �˰��� B ����ȭ�� ����� ���� ��ġ
        InitializePiles(mUrlReshuffle_withB);
        InitializePiles(mUrlRetrieval_withB);
    }

    /// <summary>
    /// ���� �����͸� �о�鿩�� ���縦 ��ġ��
    /// </summary>
    /// <param name="filePath"></param>
    /// @ note ���� �� �ݵ�� '_A'�� '_B' �� ���̾ƿ� ������ ���� ������ �־�� �մϴ�.
    private void InitializePiles(string filePath)
    {
        if (!File.Exists(filePath))
        {
            SSYManager.Log(string.Format("{0} ������ �������� �ʽ��ϴ�. \nSteel ������ �����Ͽ����ϴ�.", filePath));
            return;
        }

        LayoutType _type = (Path.GetFileName(filePath).Contains("_A")) ? LayoutType.A : LayoutType.B;
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
            GameObject _steel = Instantiate(mPrefabSteel, StockLayout.GetInstance().getExactSteelLocation(_data[mIndexPileNo]), Quaternion.identity);
            _steel.name = string.Format("{0}_{1}", _data[mIndexMarkNo], _type.ToString());
            _steel.GetComponent<Steel>().Initialize(_steelBuilder.setPileNo(_data[mIndexPileNo]).setPileSequence(_data[mIndexPileSeq]).setMarkNo(_data[mIndexMarkNo]).setUnitW(_data[mIndexUnitW]).setToPile(_data[mIndexToPile]).Build());
            StockLayout.GetInstance().PutDown(_data[mIndexPileNo], _steel.name, _type);
            _steel.transform.parent = mParentSteel.transform;
            Steels.Add(_steel);
        }
        isFirstLine = true;
    }
}
