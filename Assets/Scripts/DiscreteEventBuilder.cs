using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteEventBuilder : MonoBehaviour
{
    private DiscreteEvent mEvent = new DiscreteEvent();

    public DiscreteEventBuilder setTimestamp (string timestamp)
    {
        mEvent.Timestamp = int.Parse(timestamp);
        return this;
    }

    public DiscreteEventBuilder setMode (string mode)
    {
        Mode _mode;
        Enum.TryParse<Mode>(mode, true, out _mode);
        mEvent.State = _mode;
        return this;
    }

    public DiscreteEventBuilder setCrane (string crane)
    {
        
        mEvent.Crane = CraneManager.GetInstance().GetCrane(crane);
        return this;
    }

    public DiscreteEventBuilder setLocation (string location)
    {
        mEvent.Location = location;
        return this;
    }

    public DiscreteEventBuilder setPlate(string plate)
    {
        mEvent.Plate = plate;
        return this;
    }

    public DiscreteEvent Build()
    {
        return mEvent;
    }
}
