using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlNotDestroyableParticle : TimeControlParent
{
    // rewinding variables
    private List<PITNotDestroyableParticle> pitList = new List<PITNotDestroyableParticle>();

    // simulation states
    private ParticleSystem particleSystem;
    private float simulationTime;
    private bool currentlyStopped;


    protected override void StartInit()
    {
        // resetting particle system
        particleSystem = GetComponent<ParticleSystem>();
        if (!particleSystem.playOnAwake)
        {
            particleSystem.Stop();
        }
        simulationTime = 0;
    }

    protected override void Rewinding()
    {
        pitList[pitList.Count - 1].SetState(out simulationTime, out currentlyStopped);
        if (currentlyStopped)
        {
            particleSystem.Stop();
        }
        else
        {
            particleSystem.Simulate(simulationTime);
        }

        pitList.RemoveAt(pitList.Count - 1);
        firstFrameAfter = true;  // set first frame after to true so when R released, clear PIT list
    }

    protected override void NotRewinding()
    {
        Simulation();
        if (pitList.Count > maxRecordingTime / Time.fixedDeltaTime)
        {
            pitList.RemoveAt(0);
        }
        if (firstFrameAfter)
        {
            if (!currentlyStopped)
            {
                particleSystem.Play();
            }
            else
            {
                particleSystem.Clear();
                particleSystem.Stop();
                simulationTime = 0;
            }
            firstFrameAfter = false;
            pitList.Clear();
        }
        pitList.Add(new PITNotDestroyableParticle(simulationTime, particleSystem.isStopped));
    }

    private void Simulation()
    {
        if (particleSystem.isPlaying)
        {
            simulationTime += Time.fixedDeltaTime;
        }
        else
        {
            simulationTime = 0;
        }
    }
}
