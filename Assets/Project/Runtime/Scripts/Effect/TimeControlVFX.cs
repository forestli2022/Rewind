
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TimeControlVFX : TimeControlParent
{
    public VisualEffect reverse;  // the reverse animation of the vfx graph
    [HideInInspector]
    public List<bool> pitList;  // this pitlist record whether a vfx is destroyed at cetain time
    private bool VFXPlaying;
    private float VFXTimer;
    [SerializeField] private float VFXLifeTime;


    protected override void Rewinding()
    {
        if (pitList[pitList.Count - 1])
        {
            reverse.Play();
        }
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


        pitList.Add(false);
        if (VFXPlaying)
        {
            VFXTimer += Time.fixedDeltaTime;
            if (VFXTimer >= VFXLifeTime)
            {
                VFXPlaying = false;
                VFXTimer = 0;
                pitList[pitList.Count - 1] = true;  // destroyed this frame
            }
        }
    }

    // vfx has no isPlaying() method, so it has to be done in this intergrated way
    public void VFXIsPlaying()
    {
        VFXPlaying = true;
    }
}
