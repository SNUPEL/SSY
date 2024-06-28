using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class SteelBuilder : MonoBehaviour
{

    private Steel mSteel = new Steel();

    public SteelBuilder() { }

    public SteelBuilder setPileNo(string number)
    {
        mSteel.PileNo = number;
        return this;
    }

    public SteelBuilder setPileSequence(string sequence)
    {
        mSteel.PileSequence = int.Parse(sequence);
        return this;
    }

    public SteelBuilder setMarkNo(string id)
    {
        mSteel.MarkNo = id;
        return this;
    }

    public SteelBuilder setUnitW(string unitW)
    {
        mSteel.UnitW = float.Parse(unitW);
        return this;
    }

    public SteelBuilder setToPile(string id)
    {
        mSteel.ToPile = id;
        return this;
    }

    public Steel Build()
    {
        return mSteel;
    }
}
