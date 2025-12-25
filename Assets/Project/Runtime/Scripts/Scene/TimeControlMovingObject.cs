using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlMovingObject : TimeControlParent
{
    [Header("Moving")]
    [SerializeField] private Transform activatedPos;
    [SerializeField] private Transform deactivatedPos;
    [SerializeField] private Transform holder;
    [SerializeField] private float speed;

    private bool activated;
    List<bool> pitList = new List<bool>();

    protected override void StartInit()
    {
        holder.position = deactivatedPos.position;
    }


    protected override void Rewinding()
    {
        activated = pitList[pitList.Count - 1];
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

        if (activated && Vector3.Distance(holder.transform.position, activatedPos.position) > 0.1f)
        {
            // if not close to target, keep moving
            holder.transform.position += (activatedPos.position - holder.transform.position).normalized * speed * Time.fixedDeltaTime;
        }
        else if (!activated && Vector3.Distance(holder.transform.position, deactivatedPos.position) > 0.1f)
        {
            // if not close to original point, keep moving back
            holder.transform.position += (deactivatedPos.position - holder.transform.position).normalized * speed * Time.fixedDeltaTime;
        }
        pitList.Add(activated);
    }

    // messages
    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }
}
