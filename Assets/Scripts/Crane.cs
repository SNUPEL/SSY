using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class Crane : MonoBehaviour
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

    private bool IsHoldingSteel { get; set; }

    private StockLayout mStockLayout;
    private DiscreteEventManager mDiscreteEventManager;

    public void SetStockLayout (StockLayout stockLayout)
    {
        this.mStockLayout = stockLayout;
    }

    public void SetDiscreteEventManager (DiscreteEventManager discreteEventManager)
    {
        this.mDiscreteEventManager = discreteEventManager;
    }

    /// <summary>
    /// ũ������ ��ġ�� �����մϴ�.
    /// ũ������ x �����θ� �����̹Ƿ� pile�� ��ġ���� ������ x ���� �ٲ�ϴ�.
    /// </summary>
    /// <param name="pileNo"> �̵��� Pile �̸� </param>
    public void Locate(string pileNo)
    {
        Vector3 _to = mStockLayout.getPileLocation(pileNo);
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

        _indexCurrent = mDiscreteEventManager.Events.First(x => x.Key > _indexPrevious && x.Value.Crane.name == this.name).Key;
        _indexNext = mDiscreteEventManager.Events.First(x => x.Key > _indexCurrent && x.Value.Crane.name == this.name).Key;

        if (mDiscreteEventManager.Events.ContainsKey(_indexCurrent))
            mCurrent = mDiscreteEventManager.Events.ElementAt(_indexCurrent);
        else
            Debug.Log("���� �̺�Ʈ�� �������� �ʽ��ϴ�.");

        if (mDiscreteEventManager.Events.ContainsKey(_indexNext))
            mNext = mDiscreteEventManager.Events.ElementAt(_indexNext);
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
                Move(spent, delta);

                // ���縦 ������ ���� ���� ��� 
                if (!IsHoldingSteel)
                    this.transform.parent.GetComponent<SSYManager>().TravelingTimeWithoutSteel += delta;
                
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
                    {
                    this.transform.parent.GetComponent<SSYManager>().AvoidingWaitingTime += delta;
                    return; 
                }
                SearchNextEvent();
                break;
            case Mode.AVOIDING_WAIT_FINISH:
                SearchNextEvent();
                break;
            case Mode.WAITING_START:
                if (spent <= mNext.Value.Timestamp)
                    {
                    this.transform.parent.GetComponent<SSYManager>().WaitingTime += delta;
                    return; 
                }
                SearchNextEvent();
                break;
            case Mode.WAITING_FINISH:
                SearchNextEvent();
                break;
        }
    }

    /// <summary>
    /// �������� �̵��Ѵ�.
    /// </summary>
    /// <param name="spent"></param>
    /// <param name="delta"></param>
    private void Move(float spent, float delta)
    {
        float _distanceX = (mStockLayout.getPileLocation(mCurrent.Value.Location).x - mCenter.transform.position.x);
        float _distanceY = (mStockLayout.getPileLocation(mCurrent.Value.Location).z - mHanger.transform.position.z);
        // �������� �����߰ų� timestamp�� �Ѿ ���� �����̸� ���� Mode�� Ž���ϰ� �Ѿ
        if (Mathf.Abs(_distanceX) + Mathf.Abs(_distanceY) < 0.1 || spent >= mCurrent.Value.Timestamp)
        {
            SearchNextEvent();
            return;
        }

        Vector3 _deltaCraneMove = new Vector3();
        Vector3 _deltaHangerMove = new Vector3();
        _deltaCraneMove.x = _distanceX / (Mathf.Abs(mCurrent.Value.Timestamp - spent)) * delta;
        _deltaHangerMove.z = _distanceY / (Mathf.Abs(mCurrent.Value.Timestamp - spent)) * delta;
        this.transform.position += _deltaCraneMove;
        mHanger.transform.position += _deltaHangerMove;
    }

    private void SetTargetSteel(string steel)
    {
        mSteel = this.transform.parent.GetComponent<SSYManager>().FindSteel(steel);
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
            float _bottom = mStockLayout.GetPileHeight(mCurrent.Value.Location);
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

        IsHoldingSteel = true;
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
            float _bottom = mStockLayout.GetPileHeight(mCurrent.Value.Location);
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
                mStockLayout.AddSteel(mCurrent.Value.Location, mSteel);
                mSteel = null;
            }
            float _distance = (mUpperHeightLimit.transform.position.y - mHanger.transform.position.y);
            float _leftTime = Mathf.Abs(mWorkingTime - mSpent);
            mHanger.transform.position += new Vector3(0, _distance / _leftTime * mDelta, 0);
            return;
        }
        IsHoldingSteel = false;
        SearchNextEvent();
        mSpent = 0;
    }


    /// <summary>
    /// ũ���ο� ���� ���� ������ GUI���� ��µ˴ϴ�.
    /// </summary>
    /// @note: �θ� ������Ʈ�� �ݵ�� SSYManager�̾�� ��. �׷��� ���� ��� point ���� ����� �ҷ����� ����
    void OnGUI()
    {
        Vector3 _offset = new Vector3(0f, 60f, -100f);
        Rect _rect = new Rect(0, 0, 300, 100);
        Vector3 _point = this.transform.parent.GetComponent<SSYManager>().mCamera.WorldToScreenPoint(this.transform.position + _offset);
        _rect.x = _point.x;
        _rect.y = Screen.height - _point.y - _rect.height;

        string _log = string.Format("\nTimestamp: {0}\nState: {1}\nLocation: {2}", mDiscreteEventManager.Events[mCurrent.Key].Timestamp, mDiscreteEventManager.Events[mCurrent.Key].State, mDiscreteEventManager.Events[mCurrent.Key].Location, mDiscreteEventManager.Events[mCurrent.Key].Crane.name);
        //GUI.Label(_rect, this.name + _log);
    }
}
