using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlPistolPickup : TimeControlParent
{
    [SerializeField] private float rotatingSpeed;
    private List<PITPistolPickup> pitList = new List<PITPistolPickup>();
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject holder;

    protected override void Rewinding()
    {
        // disable pistol pickup script
        GetComponent<PistolPickup>().enabled = false;

        pitList[pitList.Count - 1].SetState(rb, out bool isActive);
        holder.SetActive(isActive);
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
            // enable pistol pickup script
            GetComponent<PistolPickup>().enabled = true;
            firstFrameAfter = false;
            pitList.Clear();
        }
        pitList.Add(new PITPistolPickup(rb, holder.activeInHierarchy));

        // rotate pistol model
        holder.transform.RotateAround(holder.transform.position, transform.up, rotatingSpeed * Time.fixedDeltaTime);
        Ray ray = new Ray(holder.transform.position, Vector3.down);
    }
}
