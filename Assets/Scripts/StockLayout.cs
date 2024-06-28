using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class StockLayout : MonoBehaviour
{
    private static StockLayout mInstance;

    public GameObject mPoint;
    private Vector3 mPilePivot = new Vector3(1938.5f, 0f, -151.5f);
    private Vector3 mB00 = new Vector3(2033.8f, 0, -100);
    private float mPileWidth = 49f;
    private float mPileHeight = 108f;
    private int xCount = 43;
    private int yCount = 2;
    private int mIndex = 0;
    private Dictionary <string, GameObject> mPiles;

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

    public Dictionary<string, GameObject> Piles
    {
        get
        {
            if (mPiles == null) mPiles = new Dictionary<string, GameObject>();
            return mPiles;
        }
    }

    /// <summary>
    /// 조선소 작업장의 레이아웃을 초기화한다.
    /// 각 파일 객체를 초기화(위치 선정, 이름 결정)
    /// </summary>
    public void InitializeLayout()
    {
        GameObject _parentPile = new GameObject("Piles");
        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                if (y == 0 && (x == 20 || x == 24 || x == 42)) 
                    continue;
                GameObject _pile = Instantiate(mPoint);
                _pile.AddComponent<Pile>();
                _pile.transform.position = place(x, y);
                _pile.name = naming(x + 1, y);
                _pile.transform.parent = _parentPile.transform;
                Piles.Add(_pile.name, _pile);
            }
        }

        // B00 추가
        GameObject _b00 = Instantiate(mPoint, _parentPile.transform, true);
        _b00.name = "B00";
        _b00.AddComponent<Pile>();
        _b00.transform.position = mB00;
        _b00.transform.parent = _parentPile.transform;
        Piles.Add(_b00.name, _b00);
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

    private Vector3 place(int i, int j)
    {
        return (mPilePivot + new Vector3(-i * mPileWidth, 0, j * mPileHeight));
    }

    public void PutDown(string pileNo, string steel)
    {
        if (!Piles[pileNo].transform.Find(steel))
        {
            return;
        }
        Piles[pileNo].GetComponent<Pile>().AddChild(steel);
    }

    /// <summary>
    /// 적치장 파일의 위치를 반환한다.
    /// </summary>
    /// <param name="pileNo">파일 번호</param>
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
    public Vector3 getExectSteelLocation(string pileNo)
    {
        if (!Piles.ContainsKey(pileNo))
            return new Vector3(-1, -1, -1);
        GameObject _pile = Piles[pileNo];
        return new Vector3(_pile.transform.position.x, _pile.GetComponent<Pile>().GetHeight(), _pile.transform.position.z);
    }

    /// <summary>
    /// 파일의 적치된 강재 총 높이를 구하여 새로운 강재를 놓을 경우 높이를 반환한다.
    /// </summary>
    /// <param name="pileNo"> 적치할 파일의 넘버</param>
    /// <returns>파일의 쌓여진 높이</returns>
    public float GetPileHeight(string pileNo)
    {
        if (!Piles.ContainsKey(pileNo))
            return -1;
        return Piles[pileNo].GetComponent<Pile>().GetHeight();
    }

    public void RemoveSteel(string pileNo, GameObject mSteel)
    {
        Piles[pileNo].GetComponent<Pile>().Remove(mSteel.name);
    }

    public void AddSteel(string pileNo, GameObject mSteel)
    {
        Piles[pileNo].GetComponent<Pile>().AddChild(mSteel.name);
    }

    public void Release(string pileNo)
    {
        if (StockLayout.GetInstance().Piles[pileNo].transform.childCount == 0) 
            return;
        StockLayout.GetInstance().Piles[pileNo].transform.GetChild(0).gameObject.SetActive(false);
        StockLayout.GetInstance().Piles[pileNo].transform.GetChild(0).transform.SetParent(SteelManager.GetInstance().ParentSteel.transform);
    }
}
