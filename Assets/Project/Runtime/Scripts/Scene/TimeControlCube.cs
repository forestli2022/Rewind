using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlCube : TimeControlSimplePhysics
{
    [SerializeField] private AudioSource cubeSound;
    public AudioSource cubeWaterSound;
    [SerializeField] private float minVelocity;
    [SerializeField] private float minAngularVelocity;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float maxAngularVelocity;

    void OnCollisionEnter(Collision collision)
    {
        // does not make sound with the player
        if (collision.gameObject.layer == 6)
        {
            return;
        }

        if (!timer.rewinding && (rb.velocity.magnitude > minVelocity || rb.angularVelocity.magnitude > minAngularVelocity))
        {
            if (!cubeSound.isPlaying)
            {
                cubeSound.volume = Mathf.Clamp(rb.velocity.magnitude / maxVelocity + rb.angularVelocity.magnitude / maxAngularVelocity, 0, 1);
                cubeSound.Play();
            }
        }
    }
}
