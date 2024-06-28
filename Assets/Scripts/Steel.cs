using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steel : MonoBehaviour
{

    private string mPileNo = string.Empty;
    private int mPileSequence;
    private string mMarkNo = string.Empty;
    private float mUnitW;
    private string mToPile = string.Empty;

    public Vector3 offset = new Vector3(0, -150f, 0);

    /// <summary>
    /// ���� ���簡 ��ġ�� ������ Number�� ��Ÿ���ϴ�.
    /// </summary>
    public string PileNo
    {
        get { return mPileNo; }
        set { mPileNo = value; }
    }
    
    /// <summary>
    /// ������ ��ġ ������ ��Ÿ���ϴ�.
    /// </summary>
    public int PileSequence
    {
        get { return mPileSequence; }
        set { mPileSequence = value; }
    }

    /// <summary>
    /// ������ ID�� ��Ÿ���ϴ�.
    /// </summary>
    public string MarkNo
    {
        get { return mMarkNo; }
        set { mMarkNo = value; }
    }

    /// <summary>
    /// ������ ���Ը� ��Ÿ���ϴ�.
    /// </summary>
    public float UnitW
    {
        get { return mUnitW; }
        set { mUnitW = value; }
    }

    /// <summary>
    /// ������ �������� ��Ÿ���ϴ�.
    /// </summary>
    public string ToPile
    {
        get { return mToPile; }
        set { mToPile = value; }
    }

    /// <summary>
    /// �ùķ��̼� ���� �� ������ �����͸� �ʱ�ȭ�մϴ�.
    /// </summary>
    /// <param name="steel"></param>
    public void Initialize(Steel steel)
    {
        this.PileNo = steel.PileNo;
        this.PileSequence = steel.PileSequence;
        this.MarkNo = steel.MarkNo;
        this.UnitW = steel.UnitW;
        this.ToPile = steel.ToPile;
    }

    //Rect rect = new Rect(0, 0, 300, 100);
    //void OnGUI()
    //{
    //    Vector3 point = Camera.main.WorldToScreenPoint(this.transform.position + offset);
    //    rect.x = point.x;
    //    rect.y = Screen.height - point.y - rect.height; // bottom left corner set to the 3D point
    //    GUI.Label(rect, this.name); // display its name, or other string
    //}
}
