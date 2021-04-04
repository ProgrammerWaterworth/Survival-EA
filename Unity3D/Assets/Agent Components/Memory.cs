using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    float timeStamp;

    public void SetTimeStamp()
    {
        timeStamp = Time.time;
    }

    public float GetTimeStamp()
    {
        return timeStamp;
    }
}
