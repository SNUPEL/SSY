using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraneManager : MonoBehaviour
{
    private static CraneManager mInstance;
    public GameObject mPrefabOverheadCrane;
    private DiscreteEventManager mDiscreteEventManager;
    private List<GameObject> mCranes;

    private const string mNullCrane = "";

    /// <summary>
    /// ũ���� ��ü�� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="crane"></param>
    /// <returns></returns>
    public Crane GetCrane(string crane)
    {
        return Cranes.First(x => x.name == crane).GetComponent<Crane>();
    }

    public void Start()
    {
    }

    /// <summary>   
    /// ũ������ �ʱ�ȭ�Ѵ�.
    /// �� ������ġ�忡���� �� ���� ũ���θ� �����ϱ� ������ ������ �ƴ� �ϵ��ڵ�� �����Ѵ�.
    /// �ݵ�� StockLayout�� ���� �ʱ�ȭ�Ǿ�� ��
    /// </summary>
    public void Initialize()
    {
        mPrefabOverheadCrane = Resources.Load<GameObject>("Prefabs/OverheadCrane");
        // Layout A�� Crane ����
        GameObject _crane_1 = Instantiate(mPrefabOverheadCrane, this.transform.parent.GetComponent<SSYManager>().mPivot + new Vector3(0, 120.4f, 76.8f), Quaternion.identity);
        _crane_1.GetComponent<Crane>().SetStockLayout(this.transform.parent.GetComponent<SSYManager>().StockLayout);
        _crane_1.GetComponent<Crane>().Locate("cn1");
        _crane_1.GetComponent<Crane>().Name = "Crane-1";
        _crane_1.name = "Crane-1";
        _crane_1.transform.SetParent(this.transform.parent);
        _crane_1.GetComponent<Crane>().SetDiscreteEventManager(mDiscreteEventManager);
        Cranes.Add( _crane_1);

        GameObject _crane_2 = Instantiate(mPrefabOverheadCrane, this.transform.parent.GetComponent<SSYManager>().mPivot + new Vector3(0, 120.4f, 76.8f), Quaternion.identity);
        _crane_2.GetComponent<Crane>().SetStockLayout(this.transform.parent.GetComponent<SSYManager>().StockLayout);
        _crane_2.GetComponent<Crane>().Locate("cn2");
        _crane_2.GetComponent<Crane>().Name = "Crane-2";
        _crane_2.name = "Crane-2";
        _crane_2.transform.SetParent(this.transform.parent);
        _crane_2.GetComponent<Crane>().SetDiscreteEventManager(mDiscreteEventManager);
        Cranes.Add(_crane_2);
        
        GameObject _crane_empty = new GameObject();
        _crane_empty.AddComponent<NullCrane>();
        _crane_empty.GetComponent<NullCrane>().Name = mNullCrane;
        _crane_empty.GetComponent<NullCrane>().SetStockLayout(this.transform.parent.GetComponent<SSYManager>().StockLayout);
        _crane_empty.name = mNullCrane;
        _crane_empty.transform.SetParent(this.transform.parent); 
        _crane_empty.GetComponent<NullCrane>().SetDiscreteEventManager(mDiscreteEventManager);
        Cranes.Add(_crane_empty);

        // Layout B�� Crane ����

    }

    public void Move(float spent, float delta)
    {
        foreach (var crane in Cranes)
            crane.GetComponent<Crane>().move(spent, delta);
    }

    public void SetDiscreteEventManager(DiscreteEventManager discreteEventManager)
    {
        this.mDiscreteEventManager = discreteEventManager;
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