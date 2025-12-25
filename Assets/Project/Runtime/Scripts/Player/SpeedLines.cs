using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLines : MonoBehaviour
{
    public Rigidbody rb;
    public ParticleSystem ps;
    public float minVelocity;
    public float maxVelocity;
    float speedRatio;

    private void Start()
    {
        speedRatio = 1f / (maxVelocity - minVelocity);
    }
    private void Update()
    {
        ps.startColor = new Color(ps.startColor.r, ps.startColor.g, ps.startColor.b, speedRatio * (rb.velocity.magnitude - minVelocity));
        ps.emissionRate = 70 + 10 * (rb.velocity.magnitude - minVelocity);
    }
}
