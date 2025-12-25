using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Transform platform;
    [SerializeField] private Transform entranceDoor;
    [SerializeField] private Transform exitDoor;
    [SerializeField] private Transform entranceDetector;
    [SerializeField] private Transform insideDetector;
    [SerializeField] private Transform elevatorStartingPoint;
    [SerializeField] private Transform elevatorFinishingPoint;
    [SerializeField] private Transform exitDoorPos;
    [SerializeField] private Transform loadPoint;  // when the y position reached loading point, load the next level

    [SerializeField] private float entraneDetectionDistance;
    [SerializeField] private float insideDetectionDistance;
    [SerializeField] private float doorSlidingDistance;
    [SerializeField] private float doorSlidingSpeed;
    [SerializeField] private float elevatorSpeed;
    [SerializeField] private LayerMask onlyPlayer;
    private GameObject gameManaging;
    private GameObject player;
    private bool inside;

    // audio
    private AudioSource[] elevatorAudios;
    private bool startPlayed = false;

    private float timeNeeded;
    private float timeCnt;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        timeNeeded = Mathf.Abs((elevatorFinishingPoint.position.y - elevatorStartingPoint.position.y)) / elevatorSpeed;
        elevatorAudios = platform.gameObject.GetComponents<AudioSource>();
        timer = GameObject.Find("Time").GetComponent<Timer>();
        gameManaging = GameObject.FindGameObjectWithTag("GameManager");

        // if player is teleported for transition, set the timer to 1 so that elevator is rising the instant player is presented.
        float loadTime = Mathf.Abs(loadPoint.position.y - elevatorStartingPoint.position.y) / elevatorSpeed + 1;
        if (platform.tag == "Respawn" && timeCnt < timeNeeded)
        {
            timeCnt = loadTime;
            startPlayed = true;
            platform.position = new Vector3(platform.position.x, loadPoint.position.y, platform.position.z);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        player = GameObject.Find("Player");
        inside = Physics.CheckSphere(insideDetector.position, insideDetectionDistance, onlyPlayer);

        // open entrance door
        if (Physics.CheckSphere(entranceDoor.position, entraneDetectionDistance, onlyPlayer) && !inside && !timer.inPast)
        {
            float desiredDoorLocation = entranceDetector.position.y + doorSlidingDistance;
            entranceDoor.position = Vector3.Lerp(entranceDoor.position, new Vector3(entranceDoor.position.x, desiredDoorLocation, entranceDoor.position.z), doorSlidingSpeed * Time.fixedDeltaTime);

            // play open door audio
            if (Mathf.Abs(entranceDoor.position.y - desiredDoorLocation) > 1 && !elevatorAudios[3].isPlaying)
            {
                elevatorAudios[4].Stop();
                elevatorAudios[3].Play();
            }
        }
        else
        {
            entranceDoor.position = Vector3.Lerp(entranceDoor.position, new Vector3(entranceDoor.position.x, entranceDetector.position.y, entranceDoor.position.z), doorSlidingSpeed * Time.fixedDeltaTime);
            if (!inside)
            {
                timeCnt = 0;
            }

            // play close door audio
            if (Mathf.Abs(entranceDoor.position.y - entranceDetector.position.y) > 1 && !elevatorAudios[4].isPlaying)
            {
                elevatorAudios[3].Stop();
                elevatorAudios[4].Play();
            }
        }

        // elevating
        if (inside && Mathf.Abs(entranceDoor.position.y - entranceDetector.position.y) < 0.05f && timeCnt < timeNeeded + 1 && Mathf.Abs(elevatorFinishingPoint.position.y - platform.position.y) > 0.05)
        {
            timeCnt += Time.fixedDeltaTime;
            platform.position = Vector3.Lerp(elevatorStartingPoint.position, elevatorFinishingPoint.position, Mathf.Max(0, (timeCnt - 1)) / timeNeeded);

            if (timeCnt > 1)
            {
                PlayElevatingAudio();
            }


            // if player walks out of elevator, clear pitlist and set rigid body to extrapolate
            player.SendMessage("ClearList");
            if (player.GetComponent<Rigidbody>().interpolation == RigidbodyInterpolation.Interpolate)
            {
                player.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Extrapolate;
            }
        }

        // open exit door
        if (inside && Mathf.Abs(elevatorFinishingPoint.position.y - platform.position.y) <= 0.05)
        {
            // keep incrementing the time
            timeCnt += Time.fixedDeltaTime;
            if (timeCnt >= timeNeeded + 2)
            {
                // if time has reached, slide the exit door down
                exitDoor.position = Vector3.Lerp(exitDoor.position, exitDoorPos.position + Vector3.down * doorSlidingDistance, doorSlidingSpeed * Time.fixedDeltaTime);
                // play open door audio
                if (Mathf.Abs(exitDoor.position.y - (exitDoorPos.position + Vector3.down * doorSlidingDistance).y) > 1 && !elevatorAudios[3].isPlaying)
                {
                    elevatorAudios[4].Stop();
                    elevatorAudios[3].Play();
                }
            }


            if (player.GetComponent<Rigidbody>().interpolation == RigidbodyInterpolation.Extrapolate)
            {
                player.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
        else
        {
            exitDoor.position = Vector3.Lerp(exitDoor.position, exitDoorPos.position, doorSlidingSpeed * Time.fixedDeltaTime);
            // play close door audio
            if (Mathf.Abs(exitDoor.position.y - exitDoorPos.position.y) > 1 && !elevatorAudios[4].isPlaying)
            {
                elevatorAudios[3].Stop();
                elevatorAudios[4].Play();
            }
        }

        // check if the load point is reached, if so, stop what is currently doing and load the next level
        if (Mathf.Abs(loadPoint.position.y - platform.position.y) <= 0.05 && platform.tag != "Respawn")
        {
            gameManaging.SendMessage("LoadNextLevel", platform.position);
        }
    }

    private void PlayElevatingAudio()
    {
        if (!startPlayed)
        {
            startPlayed = true;
            elevatorAudios[0].Play();
        }
        else
        {
            if (Mathf.Abs(elevatorFinishingPoint.position.y - platform.position.y) > 1)
            { // elevator still distant from the finishing point
                if (!elevatorAudios[0].isPlaying && !elevatorAudios[1].isPlaying)
                {
                    elevatorAudios[1].Play();
                }
            }
            else
            {
                if (!elevatorAudios[2].isPlaying)
                {
                    elevatorAudios[1].Stop();
                    elevatorAudios[2].Play();
                }
            }
        }
    }
}
