using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string mUrlResult_withA = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\result.csv";
        string mUrlReshuffle_withA = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\reshuffle_A.csv";
        string mUrlRetrieval_withA = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\retrieval_A.csv";
        Vector3 _pivot1 = new Vector3(1938.5f, 0f, -151.5f);
        //GameObject mSSYManager1 = new GameObject("SSYManager1");
        //mSSYManager1.AddComponent<SSYManager>();
        //mSSYManager1.GetComponent<SSYManager>().Initialize(mUrlReshuffle_withA, mUrlRetrieval_withA, mUrlResult_withA, _pivot1);


        string mUrlReshuffle_withB = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\reshuffle_B.csv";
        string mUrlRetrieval_withB = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\retrieval_B.csv";
        string mUrlResult_withB = "C:\\Users\\cwss0\\repos\\SSY\\Assets\\Inputs\\sim_RAND.csv";
        Vector3 _pivot2 = new Vector3(1938.1f, 0, -2119f);
        //ms2 = new GameObject("SSYManager2").AddComponent<SSYManager>().GetComponent<SSYManager>();
        //ms2.Initialize(mUrlReshuffle_withB, mUrlRetrieval_withB, mUrlResult_withA, _pivot2);
    }
}
