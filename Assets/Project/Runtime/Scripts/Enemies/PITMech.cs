using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PITMech
{
    private bool agentEnabled;
    private bool alert;
    // mech shooting, patrolling, etc.
    private float progress;
    private float lastProgress;
    private float routineProgress;
    private float timeNotSeen;
    // combat state
    private float health;
    private bool dead;
    private float secondsAfterFinished;

    public PITMech(bool agentEnabled, bool alert, float progress, float lastProgress, float routineProgress, float timeNotSeen, float health, bool dead, float secondsAfterFinished)
    {
        this.agentEnabled = agentEnabled;
        this.alert = alert;
        this.progress = progress;
        this.lastProgress = lastProgress;
        this.routineProgress = routineProgress;
        this.timeNotSeen = timeNotSeen;
        this.health = health;
        this.dead = dead;
        this.secondsAfterFinished = secondsAfterFinished;
    }

    public void SetState(out bool agentEnabled, out bool alert, out float progress, out float lastProgress, out float routineProgress, out float timeNotSeen, out float health, out bool dead, out float secondsAfterFinished)
    {
        agentEnabled = this.agentEnabled;
        alert = this.alert;
        progress = this.progress;
        lastProgress = this.lastProgress;
        routineProgress = this.routineProgress;
        timeNotSeen = this.timeNotSeen;
        health = this.health;
        dead = this.dead;
        secondsAfterFinished = this.secondsAfterFinished;
    }
}
