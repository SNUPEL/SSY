using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField]
    private string mName = string.Empty;

    /// <summary>
    /// Crane ������ ��ǥ ������ ���͸� Ŀ�������� ������.
    /// </summary>
    public GameObject mCenter;

    public GameObject mHanger;
    public GameObject mUpperHeightLimit;
    public GameObject mLowerHeightLimit;
    private GameObject mSteel;

    private KeyValuePair<int, DiscreteEvent> mPrevious;
    private KeyValuePair<int, DiscreteEvent> mCurrent;
    private KeyValuePair<int, DiscreteEvent> mNext;

    private ICraneState mStopState, mLoweringState, mLiftingState;

    private string mPileNo = string.Empty;
    private float mSpeed = 2f;

    private const float mWorkingTime = 1f;
    private float mSpent = 0;
    private float mDelta = 0;

    public string Name
    {
        get { return mName; }
        set { mName = value; }
    }

    /// <summary>
    /// ũ������ ��ġ�� �����մϴ�.
    /// ũ������ x �����θ� �����̹Ƿ� pile�� ��ġ���� ������ x ���� �ٲ�ϴ�.
    /// </summary>
    /// <param name="pileNo"> �̵��� Pile �̸� </param>
    public void Locate(string pileNo)
    {
        Vector3 _to = StockLayout.GetInstance().getPileLocation(pileNo);
        _to.y = this.gameObject.transform.position.y;
        _to.z = this.gameObject.transform.position.z;
        this.gameObject.transform.position = _to;
        mPileNo = pileNo;
    }

    public void SetPreviousEvent(KeyValuePair<int, DiscreteEvent> pair)
    {
        mPrevious = pair;
    }

    public void SetCurrentEvent(KeyValuePair<int, DiscreteEvent> pair)
    {
        mCurrent = pair;
    }

    /// <summary>
    /// ���� Mode�� Ž���ϰ� �Ѿ��.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void SearchNextEvent()
    {
        mPrevious = mCurrent;
        int _indexPrevious = mPrevious.Key;
        int _indexCurrent = 0;
        int _indexNext = 0;

        _indexCurrent = DiscreteEventManager.GetInstance().Events.First(x => x.Key > _indexPrevious && x.Value.Crane.name == this.name).Key;
        _indexNext = DiscreteEventManager.GetInstance().Events.First(x => x.Key > _indexCurrent && x.Value.Crane.name == this.name).Key;

        if (DiscreteEventManager.GetInstance().Events.ContainsKey(_indexCurrent))
            mCurrent = DiscreteEventManager.GetInstance().Events.ElementAt(_indexCurrent);
        else
            Debug.Log("���� �̺�Ʈ�� �������� �ʽ��ϴ�.");

        if (DiscreteEventManager.GetInstance().Events.ContainsKey(_indexNext))
            mNext = DiscreteEventManager.GetInstance().Events.ElementAt(_indexNext);
    }

    /**
     * @namespace
     * @brief [��Ͽ� �������� ����]
     * @note ���� ����
     * @todo �� �� ����
     * @pre �̸� ȣ���ؾ� �� ����
     * @bug ���� ����
     * @warning ���� ��ũ, ������
     * @see ������ �Լ� �Ǵ� ������
     * * @section table_sec ǥ ���� ����
* ǥ�� ������ ���� �����մϴ�.
*	�׸�								|				����
*	--------------------------------|---------------------------------
*	\ref Figure::IFigure "IFigure"	|	���� �������̽�
*	\ref Figure::Line "Line"		|	IFigure�� ��ӹ��� Line Class
     */
    public virtual void move(float spent, float delta)
    {
        switch (mCurrent.Value.State)
        {
            case Mode.MOVE_TO:
                float _distanceX = (StockLayout.GetInstance().getPileLocation(mCurrent.Value.Location).x - mCenter.transform.position.x);
                float _distanceY = (StockLayout.GetInstance().getPileLocation(mCurrent.Value.Location).z - mHanger.transform.position.z);
                // �������� �����߰ų� timestamp�� �Ѿ ���� �����̸� ���� Mode�� Ž���ϰ� �Ѿ
                if (Mathf.Abs(_distanceX) + Mathf.Abs(_distanceY) < 0.1 || spent >= mCurrent.Value.Timestamp)
                {
                    SearchNextEvent();
                    break;
                }

                Vector3 _deltaCraneMove = new Vector3();
                Vector3 _deltaHangerMove = new Vector3();
                _deltaCraneMove.x = _distanceX / (Mathf.Abs(mCurrent.Value.Timestamp - spent)) * delta;
                _deltaHangerMove.z = _distanceY / (Mathf.Abs(mCurrent.Value.Timestamp - spent)) * delta;
                this.transform.position += _deltaCraneMove;
                mHanger.transform.position += _deltaHangerMove;
                break;
            case Mode.MOVE_FROM:
                SearchNextEvent();
                break;
            case Mode.PICK_UP:
                PickUp(delta);
                break;
            case Mode.PUT_DOWN:
                PutDown(delta);
                break;
            case Mode.RELEASE:
                Release();
                SearchNextEvent();
                break;
            case Mode.AVOIDING_WAIT_START:
                if (spent <= mNext.Value.Timestamp) 
                    return;
                SearchNextEvent();
                break;
            case Mode.AVOIDING_WAIT_FINISH:
                SearchNextEvent();
                break;
            case Mode.WAITING_FINISH:
                if (spent <= mNext.Value.Timestamp)
                    return;
                SearchNextEvent();
                break;
            case Mode.WAITING_START:
                SearchNextEvent();
                break;
        }
    }

    private void SetTargetSteel(string plate)
    {
        mSteel = SteelManager.GetInstance().Steels.Find(x => x.name == plate);
    }

    /// <summary>
    /// CN���� �Ű��� ö���� Ʈ���̳� �ٸ� �������� �ű��.
    /// </summary>
    private void Release()
    {
    }

    /// <summary>
    /// ���縦 ��� �ø��� ������ �����ش�.
    /// </summary>
    private void PickUp(float delta)
    {
        if (mSteel == null)
            SetTargetSteel(mCurrent.Value.Plate);
        mDelta = delta;
        mSpent += mDelta;
        if (mSpent < mWorkingTime / 2)

        { 
            float _bottom = StockLayout.GetInstance().GetPileHeight(mCurrent.Value.Location);
            float _distance = (_bottom - mHanger.transform.position.y);
            float _leftTime = Mathf.Abs(mWorkingTime / 2 - mSpent);
            mHanger.transform.position += new Vector3(0, _distance / _leftTime * mDelta, 0);
            return;
        }
        if (mSpent >= mWorkingTime / 2 && mSpent < mWorkingTime)
        {
            if (mSteel != null)
                mSteel.transform.SetParent(mHanger.transform);
            float _distance = (mUpperHeightLimit.transform.position.y - mHanger.transform.position.y);
            float _leftTime = Mathf.Abs(mWorkingTime - mSpent);
            mHanger.transform.position += new Vector3(0, _distance / _leftTime * mDelta, 0);
            return;
        }
        SearchNextEvent();
        mSpent = 0;
    }

    /// <summary>
    /// ���縦 ���� ���� ������ �����ش�.
    /// </summary>
    private void PutDown(float delta)
    {
        mDelta = delta;
        mSpent += mDelta;

        if (mSpent < mWorkingTime / 2)
        {
            float _bottom = StockLayout.GetInstance().GetPileHeight(mCurrent.Value.Location);
            float _distance = (_bottom - mHanger.transform.position.y);
            float _leftTime = Mathf.Abs(mWorkingTime / 2 - mSpent);
            mHanger.transform.position += new Vector3(0, _distance / _leftTime * mDelta, 0);

            // �÷������Ƿ� Pile�� ���̸� ������Ʈ�Ѵ�.
            //mSteel.transform.SetParent(StockLayout.GetInstance().Piles[mNext.Value.Location].transform);
            return;
        }
        if (mSpent >= mWorkingTime / 2 && mSpent < mWorkingTime)
        {
            if (mSteel != null)
            {
                StockLayout.GetInstance().AddSteel(mCurrent.Value.Location, mSteel);
                //mSteel.transform.SetParent(null);
                mSteel = null;
            }
            float _distance = (mUpperHeightLimit.transform.position.y - mHanger.transform.position.y);
            float _leftTime = Mathf.Abs(mWorkingTime - mSpent);
            mHanger.transform.position += new Vector3(0, _distance / _leftTime * mDelta, 0);
            return;
        }
        SearchNextEvent();
        mSpent = 0;
    }


    void OnGUI()
    {
        Vector3 offset = new Vector3(0f, 60f, -100f); // height above the target position
        Rect rect = new Rect(0, 0, 300, 100);
        Vector3 point = Camera.main.WorldToScreenPoint(this.transform.position + offset);
        rect.x = point.x;
        rect.y = Screen.height - point.y - rect.height; // bottom left corner set to the 3D point

        string _log = string.Format("\nTimestamp: {0}\nState: {1}\nLocation: {2}", DiscreteEventManager.GetInstance().Events[mCurrent.Key].Timestamp, DiscreteEventManager.GetInstance().Events[mCurrent.Key].State, DiscreteEventManager.GetInstance().Events[mCurrent.Key].Location, DiscreteEventManager.GetInstance().Events[mCurrent.Key].Crane.name);
        string _log2 = " Time Spent: " + SSYManager.GetInstance().Spent;
        GUI.Label(rect, this.name + _log);
        
    }
}
