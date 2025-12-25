using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    Vector3 lastPos, lastVel;

    void FixedUpdate()
    {
        lastVel = transform.position - lastPos;
        lastPos = transform.position;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.attachedRigidbody)
        {
            col.attachedRigidbody.MovePosition(col.attachedRigidbody.position + lastVel);
        }
    }
}
