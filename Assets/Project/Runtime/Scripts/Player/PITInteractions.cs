using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct PITInteractions
{
    public List<string> actions;  // keys pressed at that frame
    public Quaternion orientation;
    public float speed;
    public Quaternion camRot;

    public PITInteractions(List<string> actions, Quaternion orientation, float speed, Quaternion camRot)
    {
        this.actions = actions;
        this.orientation = orientation;
        this.speed = speed;
        this.camRot = camRot;
    }
}
