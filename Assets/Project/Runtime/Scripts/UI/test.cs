using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    TimeControlTerminal[] terminals;
    void Start()
    {
        // get all the tertminals in the scene
        terminals = (TimeControlTerminal[])GameObject.FindObjectsOfType(typeof(TimeControlTerminal));
    }

    // Update is called once per frame
    void Update()
    {
        float minDist = float.MaxValue;
        foreach (TimeControlTerminal terminal in terminals)
        {
            float dist = Vector3.Distance(terminal.gameObject.transform.position, transform.position);
            if (minDist > dist)
            {
                minDist = dist;
            }
        }
        print(minDist);
    }
}
