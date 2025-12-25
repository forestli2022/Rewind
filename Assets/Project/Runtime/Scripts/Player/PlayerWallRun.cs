using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallRun : MonoBehaviour
{
    [Header("Wall Running")]
    public float wallDist = 0.5f;
    public float minimumJumpHeight = 1.5f;

    [HideInInspector]
    public bool wallLeft = false;
    public bool wallRight = false;

    public float g;
    public float jumpForce;
    private Rigidbody rb;
    public bool isWallRunning;
    public Transform orientation;

    public RaycastHit leftHit;
    public RaycastHit rightHit;

    [Header("Camera")]
    public Camera cam;
    public float fov;
    public float wallFov;
    public float wallFovTime;
    public float camTilt;
    public float tiltTime;

    [HideInInspector]
    public float tilt;

    public PlayerMovement playerMovement;
    public GameObject postProcessing;
    PlayerControl playerControl;

    // fixed update modifications
    bool spaceUp = false;

    // clone modification
    bool isClone;
    PITInteractions pit;
    ClonedPlayer cp;

    bool spaceLastPressed = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        isClone = gameObject.tag == "Clone";
        if (isClone)
        {
            cp = GetComponent<ClonedPlayer>();
        }
        playerControl = GetComponent<PlayerInput>().playerControl;
    }

    private void FixedUpdate()
    {
        isClone = gameObject.tag == "Clone";
        // pit for clone
        if (isClone)
        {
            pit = cp.pit;
        }

        CheckWall();

        if (canWallRun())
        {
            if (wallLeft)
            {
                StartWallRun();
            }
            else if (wallRight)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }

        // space Up
        if (!Utils.keyPressed(playerControl.Player.Jump, pit, isClone) && spaceLastPressed)
        {
            spaceUp = true;
        }
        else
        {
            spaceUp = false;
        }

        if (Utils.keyPressed(playerControl.Player.Jump, pit, isClone))
        {
            spaceLastPressed = true;
        }
        else
        {
            spaceLastPressed = false;
        }
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftHit, wallDist);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightHit, wallDist);
    }

    bool canWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void StartWallRun()
    {
        if (Utils.keyPressed(playerControl.Player.Jump, pit, isClone))
        {
            isWallRunning = true;
            rb.useGravity = false;
            rb.AddForce(Vector3.down * g, ForceMode.Force);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallFov, wallFovTime * Time.fixedDeltaTime);
        }

        if (wallLeft && Utils.keyPressed(playerControl.Player.Jump, pit, isClone))
        {
            tilt = Mathf.Lerp(tilt, -camTilt, tiltTime * Time.fixedDeltaTime);
        }
        else if (wallRight && Utils.keyPressed(playerControl.Player.Jump, pit, isClone))
        {
            tilt = Mathf.Lerp(tilt, camTilt, tiltTime * Time.fixedDeltaTime);
        }

        if (spaceUp)
        {
            if (wallLeft)
            {
                Vector3 dir = transform.up + leftHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(dir * jumpForce, ForceMode.Impulse);
            }
            else if (wallRight)
            {
                Vector3 dir = transform.up + rightHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(dir * jumpForce, ForceMode.Impulse);
            }
            spaceUp = false;
        }
    }

    void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
        if (!Utils.keyPressed(playerControl.Player.Sprint, pit, isClone) && !playerMovement.grounded)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallFovTime * Time.fixedDeltaTime);
        }
        tilt = Mathf.Lerp(tilt, 0, tiltTime * Time.fixedDeltaTime);
    }
}
