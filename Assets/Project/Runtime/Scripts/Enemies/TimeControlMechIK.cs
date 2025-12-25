using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TimeControlMechIK : TimeControlParent
{
    private MechIKFootSolver mf;
    // pitlist for mech
    private List<PITMechIK> pitList = new List<PITMechIK>();

    private int cnt = 0;
    // state variables
    private bool moved;
    private float lerp;
    private Vector3 oldPos, currentPos, newPos;
    private Vector3 oldNorm, currentNorm, newNorm;

    // Start is called before the first frame update

    protected override void StartInit()
    {
        mf = GetComponent<MechIKFootSolver>();
    }

    protected override void Rewinding()
    {
        pitList[pitList.Count - 1].SetState(out moved, out lerp, out mf.oldPos, out currentPos, out newPos, out oldNorm, out currentNorm, out newNorm);

        pitList.RemoveAt(pitList.Count - 1);
        cnt++;
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
            // assign state
            mf.moved = moved;
            mf.lerp = lerp;
            mf.oldPos = oldPos;
            mf.currentPos = currentPos;
            mf.newPos = newPos;
            mf.oldNorm = oldNorm;
            mf.currentNorm = currentNorm;
            mf.newNorm = newNorm;

            firstFrameAfter = false;
            pitList.Clear();
        }
        pitList.Add(new PITMechIK(mf.moved, mf.lerp, mf.oldPos, mf.currentPos, mf.newPos, mf.oldNorm, mf.currentNorm, mf.newNorm));

    }
}
