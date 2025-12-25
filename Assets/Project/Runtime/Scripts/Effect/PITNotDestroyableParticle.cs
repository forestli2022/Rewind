using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PITNotDestroyableParticle
{
    private float simulationTime;
    private bool stopped;

    public PITNotDestroyableParticle(float simulationTime, bool stopped)
    {
        this.simulationTime = simulationTime;
        this.stopped = stopped;
    }

    public void SetState(out float simulationTime, out bool stopped)
    {
        simulationTime = this.simulationTime;
        stopped = this.stopped;
    }
}
