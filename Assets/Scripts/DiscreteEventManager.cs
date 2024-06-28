using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DiscreteEventManager : MonoBehaviour
{
    private static DiscreteEventManager mInstance;
    private Dictionary <int, DiscreteEvent> mEvents;
    private bool isFirstLine = true;

    /// <summary>
    /// 엑셀 파일의 속성 Index
    /// </summary>
    private const int mIndexTime = 0;
    private const int mIndexEvent = 1;
    private const int mIndexCrane = 2;
    private const int mIndexLocation = 3;
    private const int mIndexPlate = 4;

    public static DiscreteEventManager GetInstance()
    {
        if (mInstance == null)
        {
            mInstance = FindObjectOfType<DiscreteEventManager>();
            if (mInstance == null)
            {
                GameObject _gameObject = new GameObject();
                _gameObject.name = typeof(DiscreteEventManager).Name;
                mInstance = _gameObject.AddComponent<DiscreteEventManager>();
            }
        }
        return mInstance;
    }

    public Dictionary <int, DiscreteEvent> Events
    {
        get
        {
            if (mEvents == null)
                mEvents = new Dictionary<int, DiscreteEvent>();
            return mEvents;
        }
    }

    public void InitializeEvents(string url)
    {
        if (!File.Exists(url))
        {
            SSYManager.Log(string.Format("{0}의 파일이 존재하지 않습니다.\nEvent 생성에 실패하였습니다.", url));
            return;
        }
        StreamReader _streamReader = new StreamReader(url);

        int _index = 0;
        while (!_streamReader.EndOfStream)
        {
            string _line = _streamReader.ReadLine();
            if (isFirstLine)
            {
                isFirstLine = false; 
                continue;
            }
            string[] _data = _line.Split(',');
            DiscreteEventBuilder _eventBuilder = new DiscreteEventBuilder();
            DiscreteEvent _event = _eventBuilder.setTimestamp(_data[mIndexTime]).setMode(_data[mIndexEvent]).setCrane(_data[mIndexCrane]).setLocation(_data[mIndexLocation]).setPlate(_data[mIndexPlate]).Build();
            Events.Add(_index++, _event);
        }
        isFirstLine = true;
    }
}
