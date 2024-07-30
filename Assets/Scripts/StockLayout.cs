using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 조선소 적치장을 관리하는 클래스
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
    /// A 적치장 내 파일을 관리합니다.
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
    /// B 적치장 내 파일을 관리합니다.
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
    /// 조선소 작업장의 레이아웃 A와 B를 초기화한다.
    /// 각 파일 객체를 초기화(위치 선정, 이름 결정)
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

        // B00 추가
        GameObject _b00 = Instantiate(_point, _parentPile.transform, true);
        _b00.name = "B00";
        _b00.AddComponent<Pile>();
        _b00.transform.position = this.transform.parent.GetComponent<SSYManager>().mPivot + new Vector3(95.7f, 0, 108f);
        _b00.transform.parent = _parentPile.transform;
        Piles.Add(_b00.name, _b00);

        // A00 추가
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
    /// 시뮬레이션 시작 전, 강재를 파일 위에 둔다.
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
    public Vector3 getExactSteelLocation(string pileNo)
    {
        if (!Piles.ContainsKey(pileNo) && !PilesB.ContainsKey(pileNo))
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

    public void AddSteel(string pileNo, GameObject mSteel)
    {
        mSteel.transform.SetParent(Piles[pileNo].transform);
    }

    /// <summary>
    /// 이동이 끝난 강재를 반출한다.
    /// 가시성을 
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
