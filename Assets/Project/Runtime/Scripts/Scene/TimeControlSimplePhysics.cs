using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlSimplePhysics : TimeControlParent
{
    protected Rigidbody rb;
    List<RigidbodyState> pitList = new List<RigidbodyState>();

    protected override void StartInit()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Rewinding()
    {
        rb.isKinematic = true;
        pitList[pitList.Count - 1].SetRigidbody(rb);
        pitList.RemoveAt(pitList.Count - 1);
        firstFrameAfter = true;
    }

    protected override void NotRewinding()
    {
        if (pitList.Count > maxRecordingTime / Time.fixedDeltaTime)
        {
            pitList.RemoveAt(0);
        }
        if (firstFrameAfter)
        {
            firstFrameAfter = false;
            pitList.Clear();
        }
        pitList.Add(new RigidbodyState(rb));
    }
}
