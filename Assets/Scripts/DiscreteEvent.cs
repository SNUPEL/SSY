using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteEvent : MonoBehaviour
{
    private float mTimestamp;
    private Mode mMode;
    private Crane mCrane;
    private string mLocation;
    private string mPlate;

    public float Timestamp
    {
        get { return mTimestamp; }
        set { mTimestamp = value; }
    }

    public Mode State
    {
        get { return mMode; }
        set { mMode = value; }
    }
    
    public Crane Crane
    {
        get { return mCrane; }
        set { mCrane = value; }
    }

    public string Location
    {
        get { return mLocation; }
        set { mLocation = value; }
    }

    public string Plate
    {
        get { return mPlate; }
        set { mPlate = value; }
    }
}
