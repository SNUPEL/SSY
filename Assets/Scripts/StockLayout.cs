using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ������ ��ġ���� �����ϴ� Ŭ����
/// </summary>
public class StockLayout : MonoBehaviour
{
    private static StockLayout mInstance;

    public GameObject mPoint;
    private Vector3 mPileAPivot = new Vector3(1938.5f, 0f, -151.5f);
    private Vector3 mPileBPivot = new Vector3();
    private Vector3 mB00 = new Vector3(2033.8f, 0, -100);
    private float mPileWidth = 49f;
    private float mPileHeight = 108f;
    private int xCount = 43;
    private int yCount = 2;
    private int mIndex = 0;
    private Dictionary<string, GameObject> mPilesA;
    private Dictionary<string, GameObject> mPilesB;

    public static StockLayout GetInstance()
    {
        if (mInstance == null)
        {
            mInstance = FindObjectOfType<StockLayout>();
            if (mInstance == null )
            {
                GameObject _object = new GameObject();
                _object.name = typeof(StockLayout).Name;
                mInstance = _object.AddComponent<StockLayout>();
            }
        }
        return mInstance;
    }

    /// <summary>
    /// A ��ġ�� �� ������ �����մϴ�.
    /// </summary>
    public Dictionary<string, GameObject> PilesA
    {
        get
        {
            if (mPilesA == null) mPilesA = new Dictionary<string, GameObject>();
            return mPilesA;
        }
    }

    /// <summary>
    /// B ��ġ�� �� ������ �����մϴ�.
    /// </summary>
    public Dictionary <string, GameObject > PilesB
    {
        get
        {
            if (mPilesB == null) mPilesB = new Dictionary<string, GameObject>();
            return mPilesB;
        }
    }

    /// <summary>
    /// ������ �۾����� ���̾ƿ� A�� B�� �ʱ�ȭ�Ѵ�.
    /// �� ���� ��ü�� �ʱ�ȭ(��ġ ����, �̸� ����)
    /// </summary>
    public void Initialize()
    {
        InitializeLayout(mPileAPivot);
        InitializeLayout(mPileBPivot);
    }

    private void InitializeLayout(Vector3 pivot)
    {
        string _pileName = (pivot == mPileAPivot) ? "PilesA" : "PilesB";
        GameObject _parentPile = new GameObject(_pileName);
        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                if (y == 0 && (x == 20 || x == 24 || x == 42))
                    continue;
                GameObject _pile = Instantiate(mPoint);
                _pile.AddComponent<Pile>();
                _pile.transform.position = place(x, y, pivot);
                _pile.name = naming(x + 1, y);
                _pile.transform.parent = _parentPile.transform;
                if (pivot == mPileAPivot)
                    PilesA.Add(_pile.name, _pile);
                else
                    PilesB.Add(_pile.name, _pile);
            }
        }

        // B00 �߰�
        GameObject _b00 = Instantiate(mPoint, _parentPile.transform, true);
        _b00.name = "B00";
        _b00.AddComponent<Pile>();
        _b00.transform.position = mB00;
        _b00.transform.parent = _parentPile.transform;
        if (pivot == mPileAPivot)
            PilesA.Add(_b00.name, _b00);
        else
            PilesB.Add(_b00.name, _b00);
    }

    private string naming(int x, int y)
    {
        if (x == 43)
            return "cn3";
        if (x >= 25)
        {
            if (x == 25)
                return "cn2";
            else
                return string.Format("{0}{1}", (y == 0) ? "A" : "B", x - 2);
        }
        if (x >= 21)
        {
            if (x == 21)
                return "cn1";
            else
                return string.Format("{0}{1}", (y == 0) ? "A" : "B", x - 1);
        }
        return string.Format("{0}{1:00}", (y == 0) ? "A" : "B", x);
    }

    private Vector3 place(int i, int j, Vector3 pivot)
    {
        return (pivot + new Vector3(-i * mPileWidth, 0, j * mPileHeight));
    }

    public void PutDown(string pileNo, string steel, LayoutType type)
    {
        switch (type)
        {
            case LayoutType.A:
                if (!PilesA[pileNo].transform.Find(steel))
                    return;
                PilesA[pileNo].GetComponent<Pile>().AddChild(steel);
                break;
            case LayoutType.B:
                if (!PilesB[pileNo].transform.Find(steel))
                    return;
                PilesB[pileNo].GetComponent<Pile>().AddChild(steel);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ��ġ�� ������ ��ġ�� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="pileNo">���� ��ȣ</param>
    /// <returns></returns>
    public Vector3 getPileLocation(string pileNo)
    {
        if (!PilesA.ContainsKey(pileNo))
            return new Vector3(-1, -1, -1);
        return PilesA[pileNo].transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pileNo"></param>
    /// <returns></returns>
    public Vector3 getExactSteelLocation(string pileNo)
    {
        if (!PilesA.ContainsKey(pileNo))
            return new Vector3(-1, -1, -1);
        GameObject _pile = PilesA[pileNo];
        return new Vector3(_pile.transform.position.x, _pile.GetComponent<Pile>().GetHeight(), _pile.transform.position.z);
    }

    /// <summary>
    /// ������ ��ġ�� ���� �� ���̸� ���Ͽ� ���ο� ���縦 ���� ��� ���̸� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="pileNo"> ��ġ�� ������ �ѹ�</param>
    /// <returns>������ �׿��� ����</returns>
    public float GetPileHeight(string pileNo)
    {
        if (!PilesA.ContainsKey(pileNo))
            return -1;
        return PilesA[pileNo].GetComponent<Pile>().GetHeight();
    }

    public void RemoveSteel(string pileNo, GameObject mSteel)
    {
        PilesA[pileNo].GetComponent<Pile>().Remove(mSteel.name);
    }

    public void AddSteel(string pileNo, GameObject mSteel)
    {
        PilesA[pileNo].GetComponent<Pile>().AddChild(mSteel.name);
    }

    public void Release(string pileNo)
    {
        if (StockLayout.GetInstance().PilesA[pileNo].transform.childCount == 0) 
            return;
        StockLayout.GetInstance().PilesA[pileNo].transform.GetChild(0).gameObject.SetActive(false);
        StockLayout.GetInstance().PilesA[pileNo].transform.GetChild(0).transform.SetParent(SteelManager.GetInstance().ParentSteel.transform);
    }
}
