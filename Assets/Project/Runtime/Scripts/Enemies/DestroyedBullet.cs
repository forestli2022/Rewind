using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedBullet
{
    public Vector3 position;
    public float existingTime;
    public Vector3 dir;
    public DestroyedBullet(Vector3 position, Vector3 dir, float existingTime){
        this.position = position;
        this.dir = dir;
        this.existingTime = existingTime;
    }
}
