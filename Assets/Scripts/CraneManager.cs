using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraneManager : MonoBehaviour
{
    private static CraneManager mInstance;
    public GameObject mPrefabOverheadCrane;
    private List<GameObject> mCranes;

    private const string mNullCrane = "";

    public static CraneManager GetInstance()
    {
        if (mInstance == null)
        {
            mInstance = FindObjectOfType<CraneManager>();
            if (mInstance == null)
            {
                GameObject _gameObject = new GameObject();
                _gameObject.name = typeof(CraneManager).Name;
                mInstance = _gameObject.AddComponent<CraneManager>();
            }
        }
        return mInstance;
    }

    /// <summary>
    /// ũ���� ��ü�� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="crane"></param>
    /// <returns></returns>
    public CraneController GetCrane(string crane)
    {
        return Cranes.First(x => x.name == crane).GetComponent<CraneController>();
    }

    /// <summary>
    /// ũ������ �ʱ�ȭ�Ѵ�.
    /// �� ������ġ�忡���� �� ���� ũ���θ� �����ϱ� ������ ������ �ƴ� �ϵ��ڵ�� �����Ѵ�.
    /// �ݵ�� StockLayout�� ���� �ʱ�ȭ�Ǿ�� ��
    /// </summary>
    public void Initialize()
    {
        GameObject _crane_1 = Instantiate(mPrefabOverheadCrane);
        _crane_1.GetComponent<CraneController>().Locate("cn1");
        _crane_1.GetComponent<CraneController>().Name = "Crane-1";
        _crane_1.name = "Crane-1";
        Cranes.Add( _crane_1);
        GameObject _crane_2 = Instantiate(mPrefabOverheadCrane);
        _crane_2.GetComponent<CraneController>().Locate("cn2");
        _crane_2.GetComponent<CraneController>().Name = "Crane-2";
        _crane_2.name = "Crane-2";
        Cranes.Add(_crane_2);
        GameObject _crane_empty = new GameObject();
        _crane_empty.AddComponent<NullCraneController>();
        _crane_empty.GetComponent<NullCraneController>().Name = mNullCrane;
        _crane_empty.name = mNullCrane;
        Cranes.Add(_crane_empty);
    }

    public void Move(float spent, float delta)
    {
        foreach (var crane in Cranes)
            crane.GetComponent<CraneController>().move(spent, delta);
    }

    public List<GameObject> Cranes
    {
        get
        {
            if (mCranes == null) mCranes = new List<GameObject>();
            return mCranes;
        }
        set { }
    }
}