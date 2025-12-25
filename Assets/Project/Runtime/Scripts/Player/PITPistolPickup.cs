using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PITPistolPickup
{
    private RigidbodyState rbState;
    private bool objectActive;

    public PITPistolPickup(Rigidbody rb, bool objectActive)
    {
        rbState = new RigidbodyState(rb);
        this.objectActive = objectActive;
    }

    public void SetState(Rigidbody rb, out bool objectActive)
    {
        rbState.SetRigidbody(rb);
        objectActive = this.objectActive;
    }
}
