using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlNonPhysics : TimeControlParent
{
    private List<PITNonPhysics> pitList = new List<PITNonPhysics>();  // using a list is actually found to be faster than pushing elements to a stack in c#. Although it leads to less expressive code, it can increase the performance of the game

    protected override void Rewinding()
    {
        pitList[pitList.Count - 1].SetState(transform);
        pitList.RemoveAt(pitList.Count - 1);
        firstFrameAfter = true;  // set first frame after to true so when R released, clear PIT list
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
        pitList.Add(new PITNonPhysics(transform.localPosition, transform.localRotation, transform.localScale));
    }
}
