using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sample : MonoBehaviour
{
    [System.Serializable]
    public class SubClass
    {
        public int         sampleInt;
        public UnityEvent  sampleEvent;
        public List<int>   sampleList;        
    }

    public GameObject     sampleObject;
    public bool           sampleBool;
    public int            sampleInt;
    public float          sampleFloat;
    public string         sampleString;
    public Vector3        sampleVector;
    public Color          sampleColor;
    public AnimationCurve sampleCurve;
    public UnityEvent     sampleEvent;
    public List<int>      sampleList;
    public CameraType     sampleEnum;
    public SubClass       sampleSubClass;
    public List<SubClass> sampleSubClassList;
}