using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float playerHeight = 2f;
    [SerializeField] private PlayerWallRun wallRun;
    [SerializeField] private PlayerClimb playerClimb;
    [SerializeField] private Transform orientation;
    [SerializeField] private Camera cam;
    [SerializeField] private CapsuleCollider collider;


    [Header("Movement")]
    public float speed = 0f;
    [SerializeField] private float movementMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Sprint")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float fov;
    [SerializeField] private float sprintFov;
    [SerializeField] private float sprintTime;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private float crouchedZ;
    [HideInInspector] public bool crouched = false;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;
    [SerializeField] private float crouchDrag = 0.5f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground")]
    [HideInInspector] public bool grounded;
    [SerializeField] private float groundDist = 0.4f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;


    private Vector3 moveDirection;

    private Rigidbody rb;

    private RaycastHit slopeHit;
    private Vector3 slopeMoveDirection;
    private PlayerControl playerControl;

    // clone modification
    private bool isClone;
    private PITInteractions pit;
    private ClonedPlayer cp;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isClone = gameObject.tag == "Clone";
        if (isClone)
        {
            cp = GetComponent<ClonedPlayer>();
        }
        else
        {
            // move player to the spawn point for seamless transition
            GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
            transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position + gameManager.GetComponent<GameManaging>().playerToPlatformPos + Vector3.up * 1.5f;
        }
    }

    private void FixedUpdate()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;

        isClone = gameObject.tag == "Clone";
        // pit for clone
        if (isClone)
        {
            pit = cp.pit;
        }

        grounded = Physics.CheckSphere(groundCheck.position, groundDist, groundLayer);

        MovementInput();

        ControlDrag();
        ControlSpeed();

        if (Utils.keyPressed(playerControl.Player.Jump, pit, isClone) && grounded)
        {
            Jump();
        }

        if (Utils.keyPressed(playerControl.Player.Slide, pit, isClone) && (Physics.Raycast(transform.position, Vector3.down, 1f) || OnSlope() || grounded))
        {
            Crouch();
        }
        else
        {
            // stop crouching
            collider.direction = 1;
            collider.center = new Vector3(0, collider.center.y, 0);
            collider.height = 1.71f;
            collider.radius = 0.35f;
            crouched = false;
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        Move();
    }


    void MovementInput()
    {
        verticalMovement = 0;
        horizontalMovement = 0;
        if (Utils.keyPressed(playerControl.Player.MoveForward, pit, isClone))
        {
            verticalMovement += 1;
        }
        if (Utils.keyPressed(playerControl.Player.MoveBackward, pit, isClone))
        {
            verticalMovement -= 1;
        }
        if (Utils.keyPressed(playerControl.Player.MoveRight, pit, isClone))
        {
            horizontalMovement += 1;
        }
        if (Utils.keyPressed(playerControl.Player.MoveLeft, pit, isClone))
        {
            horizontalMovement -= 1;
        }

        moveDirection = (orientation.forward * verticalMovement + orientation.right * horizontalMovement).normalized;
    }

    void Move()
    {
        if ((grounded || crouched) && !OnSlope())
        {
            rb.AddForce(moveDirection * movementMultiplier * speed, ForceMode.Acceleration);
        }
        else if (grounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection * movementMultiplier * speed, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveDirection * movementMultiplier * speed * airMultiplier, ForceMode.Acceleration);
        }
    }

    void ControlDrag()
    {
        if (crouched)
        {
            rb.drag = crouchDrag;
        }
        else if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }

    }

    void Jump()
    {
        float v = rb.velocity.magnitude;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * (jumpForce + v * 0.3f), ForceMode.Impulse);
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    void ControlSpeed()
    {
        if (Utils.keyPressed(playerControl.Player.Sprint, pit, isClone) && grounded && !crouched)
        {
            speed = Mathf.Lerp(speed, sprintSpeed, acceleration * Time.fixedDeltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFov, sprintTime * Time.fixedDeltaTime);
        }
        else if (crouched)
        {
            speed = Mathf.Lerp(speed, crouchSpeed, 10 * Time.fixedDeltaTime);
        }
        else
        {
            speed = Mathf.Lerp(speed, walkSpeed, acceleration * Time.fixedDeltaTime);
            if (!wallRun.isWallRunning)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, sprintTime * Time.fixedDeltaTime);
            }
        }
    }

    void Crouch()
    {
        if (rb.velocity.magnitude > 5f && Vector3.Dot(rb.velocity, orientation.forward) > 0)
        {
            collider.direction = 2;
            collider.center = new Vector3(0, collider.center.y, crouchedZ);
            collider.radius = 0.25f;
            crouched = true;
        }
    }
}
