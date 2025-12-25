using UnityEngine;
using System.Collections;

public struct RigidbodyState
{
    public float drag;
    public bool freezeRotation;
    public bool isKinematic;
    public float mass;
    public Vector3 position;
    public Quaternion rotation;
    public bool useGravity;
    public Vector3 velocity;


    public RigidbodyState(Rigidbody rb)
    {
        drag = rb.drag;
        freezeRotation = rb.freezeRotation;
        isKinematic = rb.isKinematic;
        mass = rb.mass;
        position = rb.position;
        rotation = rb.rotation;
        useGravity = rb.useGravity;
        velocity = rb.velocity;
    }

    public void SetRigidbody(Rigidbody rb)
    {
        rb.drag = drag;
        rb.freezeRotation = freezeRotation;
        rb.isKinematic = isKinematic;
        rb.mass = mass;
        rb.position = position;
        rb.rotation = rotation;
        rb.useGravity = useGravity;
        rb.velocity = velocity;
    }
}
