using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    // message from activation area
    public void Activate()
    {
        GetComponent<AudioSource>().Play();
    }
}
