using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimeControlParent : MonoBehaviour
{
    protected GameObject time;
    protected float maxRecordingTime;

    protected bool firstFrameAfter = false;

    protected Timer timer;


    protected void Start()
    {
        time = GameObject.Find("Time");
        maxRecordingTime = time.GetComponent<Timer>().maximumRecordingTime;
        timer = time.GetComponent<Timer>();
        StartInit();
    }

    protected virtual void FixedUpdate()
    {
        if (timer.rewinding)
        {
            Rewinding();
        }
        else
        {
            NotRewinding();
        }
    }

    protected virtual void StartInit()
    {

    }

    protected abstract void Rewinding();

    protected abstract void NotRewinding();
}
