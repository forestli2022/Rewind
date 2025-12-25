using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    private Timer timer;
    [SerializeField] private float gateSlidingSpeed;
    [SerializeField] private TimeControlDoor leftSideScript;
    [SerializeField] private TimeControlDoor rightSideScript;

    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private Transform leftDoorTarget;
    [SerializeField] private Transform rightDoorTarget;
    [SerializeField] private Transform leftDoorOrigin;
    [SerializeField] private Transform rightDoorOrigin;


    void Start()
    {
        timer = GameObject.Find("Time").GetComponent<Timer>();
    }

    void FixedUpdate()
    {
        if (timer.rewinding)
        {
            return;  // do nothing
        }

        // gate logic
        // opening
        if (leftSideScript.GetComponent<TimeControlDoor>().opening || rightSideScript.GetComponent<TimeControlDoor>().opening)
        {
            leftDoor.transform.position = Vector3.Lerp(leftDoor.transform.position, leftDoorTarget.position, gateSlidingSpeed * Time.fixedDeltaTime);
            rightDoor.transform.position = Vector3.Lerp(rightDoor.transform.position, rightDoorTarget.position, gateSlidingSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // door close
            leftDoor.transform.position = Vector3.Lerp(leftDoor.transform.position, leftDoorOrigin.position, gateSlidingSpeed * Time.fixedDeltaTime);
            rightDoor.transform.position = Vector3.Lerp(rightDoor.transform.position, rightDoorOrigin.position, gateSlidingSpeed * Time.fixedDeltaTime);
        }
    }
}
