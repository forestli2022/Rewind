using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechRoutine : MonoBehaviour
{ 
    public float angleRange = 120f;
    [HideInInspector]
    public float routineProgress = 0f;
    public float timeForOneCycle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // adjust turret position;
        if(routineProgress < timeForOneCycle / 2){
            transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0f, Time.fixedDeltaTime),
            Mathf.LerpAngle(transform.eulerAngles.y, angleRange / 2, Time.fixedDeltaTime),
            Mathf.LerpAngle(transform.eulerAngles.z, 0f, Time.fixedDeltaTime));
        }else{
            transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0f, Time.fixedDeltaTime),
            Mathf.LerpAngle(transform.eulerAngles.y, -angleRange / 2, Time.fixedDeltaTime),
            Mathf.LerpAngle(transform.eulerAngles.z, 0f, Time.fixedDeltaTime));
        }
        routineProgress += Time.fixedDeltaTime;
        if(routineProgress > timeForOneCycle){
            routineProgress = 0;
        }
    }
}
