using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PITMechIK
{
    private bool moved;
    private float lerp;
    private Vector3 oldPos, currentPos, newPos;
    private Vector3 oldNorm, currentNorm, newNorm;

    public PITMechIK(bool moved, float lerp, Vector3 oldPos, Vector3 currentPos, Vector3 newPos, Vector3 oldNorm, Vector3 currentNorm, Vector3 newNorm)
    {
        this.moved = moved;
        this.lerp = lerp;
        this.oldPos = oldPos;
        this.currentPos = currentPos;
        this.newPos = newPos;
        this.oldNorm = oldNorm;
        this.currentNorm = currentNorm;
        this.newNorm = newNorm;
    }

    public void SetState(out bool moved, out float lerp, out Vector3 oldPos, out Vector3 currentPos, out Vector3 newPos, out Vector3 oldNorm, out Vector3 currentNorm, out Vector3 newNorm)
    {
        moved = this.moved;
        lerp = this.lerp;
        oldPos = this.oldPos;
        currentPos = this.currentPos;
        newPos = this.newPos;
        oldNorm = this.oldNorm;
        currentNorm = this.currentNorm;
        newNorm = this.newNorm;
    }
}
