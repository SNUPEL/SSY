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
    private float mPileWidth = 49f;
    private float mPileHeight = 108f;
    private int xCount = 43;
    private int yCount = 2;
    private int mIndex = 0;
    private Dictionary<string, GameObject> mPilesA;
    private Dictionary<string, GameObject> mPilesB;


    /// <summary>
    /// A ��ġ�� �� ������ �����մϴ�.
    /// </summary>
    public Dictionary<string, GameObject> Piles
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
    public void Initialize(Vector3 pivot)
    {
        InitializeLayout(pivot);
    }

    private void InitializeLayout(Vector3 pivot)
    {
        GameObject _parentPile = new GameObject("Piles");
        GameObject _point = Resources.Load<GameObject>("Prefabs/point");
        _parentPile.transform.SetParent(this.transform.parent);
        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                if (y == 0 && (x == 20 || x == 24 || x == 42))
                    continue;
                GameObject _pile = Instantiate(_point, _parentPile.transform, true);
                _pile.AddComponent<Pile>();
                _pile.transform.position = place(x, y, pivot);
                _pile.name = naming(x + 1, y);
                _pile.transform.parent = _parentPile.transform;
                Piles.Add(_pile.name, _pile);
            }
        }

        // B00 �߰�
        GameObject _b00 = Instantiate(_point, _parentPile.transform, true);
        _b00.name = "B00";
        _b00.AddComponent<Pile>();
        _b00.transform.position = this.transform.parent.GetComponent<SSYManager>().mPivot + new Vector3(95.7f, 0, 108f);
        _b00.transform.parent = _parentPile.transform;
        Piles.Add(_b00.name, _b00);

        // A00 �߰�
        GameObject _a00 = Instantiate(_point, _parentPile.transform, true);
        _a00.name = "A00";
        _a00.AddComponent<Pile>();
        _a00.transform.position = this.transform.parent.GetComponent<SSYManager>().mPivot + new Vector3(95.7f, 0, 0);
        _a00.transform.parent = _parentPile.transform;
        Piles.Add(_a00.name, _a00);

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

    /// <summary>
    /// �ùķ��̼� ���� ��, ���縦 ���� ���� �д�.
    /// </summary>
    /// <param name="pileNo"></param>
    /// <param name="steel"></param>
    /// <param name="type"></param>
    public void InitializeSteels(string pileNo, GameObject steel)
    {
        if (!Piles[pileNo].transform.Find(steel.name))
            return;
        steel.transform.SetParent(Piles[pileNo].transform, false);
    }

    /// <summary>
    /// ��ġ�� ������ ��ġ�� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="pileNo">���� ��ȣ</param>
    /// <returns></returns>
    public Vector3 getPileLocation(string pileNo)
    {
        if (!Piles.ContainsKey(pileNo))
            return new Vector3(-1, -1, -1);
        return Piles[pileNo].transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pileNo"></param>
    /// <returns></returns>
    public Vector3 getExactSteelLocation(string pileNo)
    {
        if (!Piles.ContainsKey(pileNo) && !PilesB.ContainsKey(pileNo))
            return new Vector3(-1, -1, -1);
        GameObject _pile = Piles[pileNo];
        return new Vector3(_pile.transform.position.x, _pile.GetComponent<Pile>().GetHeight(), _pile.transform.position.z);
    }

    /// <summary>
    /// ������ ��ġ�� ���� �� ���̸� ���Ͽ� ���ο� ���縦 ���� ��� ���̸� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="pileNo"> ��ġ�� ������ �ѹ�</param>
    /// <returns>������ �׿��� ����</returns>
    public float GetPileHeight(string pileNo)
    {
        if (!Piles.ContainsKey(pileNo))
            return -1;
        return Piles[pileNo].GetComponent<Pile>().GetHeight();
    }

    public void AddSteel(string pileNo, GameObject mSteel)
    {
        mSteel.transform.SetParent(Piles[pileNo].transform);
    }

    /// <summary>
    /// �̵��� ���� ���縦 �����Ѵ�.
    /// ���ü��� 
    /// </summary>
    /// <param name="pileNo"></param>
    public void Release(string pileNo)
    {
        if (Piles[pileNo].transform.childCount == 0) 
            return;
        Piles[pileNo].transform.GetChild(0).gameObject.SetActive(false);
        Piles[pileNo].transform.GetChild(0).transform.SetParent(this.transform.parent.GetComponent<SSYManager>().GetParentSteel().transform);
    }
}
