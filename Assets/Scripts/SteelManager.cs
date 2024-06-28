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

    public void InitializePiles(string url)
    {
        if (!File.Exists(url))
        {
            SSYManager.Log(string.Format("{0} 파일이 존재하지 않습니다. \nSteel 생성에 실패하였습니다.", url));
            return;
        }
        StreamReader _streamReader = new StreamReader(url);

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
            GameObject _steel = Instantiate(mPrefabSteel, StockLayout.GetInstance().getExectSteelLocation(_data[mIndexPileNo]), Quaternion.identity);
            _steel.name = _data[mIndexMarkNo];
            _steel.GetComponent<Steel>().Initialize(_steelBuilder.setPileNo(_data[mIndexPileNo]).setPileSequence(_data[mIndexPileSeq]).setMarkNo(_data[mIndexMarkNo]).setUnitW(_data[mIndexUnitW]).setToPile(_data[mIndexToPile]).Build());
            StockLayout.GetInstance().PutDown(_data[mIndexPileNo], _data[mIndexMarkNo]);
            _steel.transform.parent = mParentSteel.transform;
            Steels.Add(_steel);
        }
        isFirstLine = true;
    }
}
