using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pile : MonoBehaviour
{
    private List<string> mSteels;
    private const float steelHeight = 0.05f;

    public float GetHeight()
    {
        return this.gameObject.transform.childCount * steelHeight;
    }

    public void AddChild(string name)
    {
        if (this.gameObject.transform.Find(name) == null)
        {
            SteelManager.GetInstance().Steels.First(x => x.name == name).transform.SetParent(this.gameObject.transform);
        }
            
    }
    public void Remove(string name) 
    {
        if (this.gameObject.transform.Find(name) != null)
        {
            SteelManager.GetInstance().Steels.First(x => x.name == name).transform.SetParent(null);
        }
    }
}
