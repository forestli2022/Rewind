using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PITNonPhysics
{
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;


    public PITNonPhysics(Vector3 p, Quaternion r, Vector3 s)
    {
        this.position = p;
        this.rotation = r;
        this.scale = s;
    }

    public void SetState(Transform t)  // set transform
    {
        t.localPosition = this.position;
        t.localRotation = this.rotation;
        t.localScale = this.scale;
    }
}
