using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 6)
        {
            // instantly kill player
            GameObject.Find("Player").SendMessage("ReduceHealth", 10);
        }
        else if (col.gameObject.tag == "Cube")  // cube contact with water
        {
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            rb.drag = 10;
            rb.angularDrag = 10;
            TimeControlCube timeControlCube = col.gameObject.GetComponent<TimeControlCube>();
            if (!timeControlCube.cubeWaterSound.isPlaying)
            {
                timeControlCube.cubeWaterSound.Play();
            }
        }
    }
}
