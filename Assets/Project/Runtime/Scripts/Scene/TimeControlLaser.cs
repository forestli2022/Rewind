using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlLaser : TimeControlParent
{
    [SerializeField] private bool inverted;
    [SerializeField] private Transform startPoint;
    [SerializeField] private float maxDist;
    private LineRenderer lr;
    [SerializeField] private bool activated = false;

    private List<bool> pitList = new List<bool>();  // record if the laser is activated
    protected override void StartInit()
    {
        lr = GetComponent<LineRenderer>();
    }

    protected override void Rewinding()
    {
        activated = pitList[pitList.Count - 1];
        pitList.RemoveAt(pitList.Count - 1);
        firstFrameAfter = true;  // set first frame after to true so when R released, clear PIT list

        // laser logic (since touching laser in the rewinding state also counts as death, so the logic musrt also be applied here)
        if (!activated)
        {
            return;
        }

        if (Physics.Raycast(startPoint.position, transform.up, out RaycastHit hit, maxDist))
        {
            if (hit.transform.gameObject.layer == 6)
            {
                GameObject.Find("Player").GetComponent<PlayerCombatState>().health = 0;
            }
            lr.SetPosition(1, hit.point);
        }
        else
        {
            lr.SetPosition(1, transform.up * maxDist);
        }
        lr.SetPosition(0, startPoint.position);
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
        pitList.Add(activated);

        // laser logic
        if (!activated)
        {
            lr.SetPosition(1, transform.position);
            return;
        }

        if (Physics.Raycast(startPoint.position, transform.up, out RaycastHit hit, maxDist))
        {
            if (hit.transform.gameObject.layer == 6)
            {
                GameObject.Find("Player").SendMessage("ReduceHealth", 10);
            }
            lr.SetPosition(1, hit.point);
        }
        else
        {
            lr.SetPosition(1, transform.up * maxDist);
        }
        lr.SetPosition(0, startPoint.position);
    }

    // messages
    public void Activate()
    {
        activated = !inverted;
    }

    public void Deactivate()
    {
        activated = inverted;
    }
}
