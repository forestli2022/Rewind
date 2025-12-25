using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClimb : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private bool down = false;
    private bool up = false;
    private bool mid = false;
    [SerializeField] private Transform player;
    [SerializeField] private Transform orientation;
    [SerializeField] private float wallDist = 0.6f;
    [SerializeField] private bool lowClimbEnabled;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private float downPos;
    [SerializeField] private float upPos;
    [SerializeField] private float midPos;
    [HideInInspector] public bool climbing;
    [SerializeField] private float climbSpeed;

    [HideInInspector] public bool highClimb;
    [SerializeField] private PlayerMovement playerMovement;

    // clone modification
    private bool isClone;
    private PITInteractions pit;
    private ClonedPlayer cp;
    // input
    private PlayerControl playerControl;

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

        down = Physics.Raycast(orientation.position + new Vector3(0, downPos, 0), orientation.forward, wallDist, wallLayer);
        mid = Physics.Raycast(orientation.position + new Vector3(0, midPos, 0), orientation.forward, wallDist, wallLayer);
        up = Physics.Raycast(orientation.position + new Vector3(0, upPos, 0), orientation.forward, wallDist, wallLayer);

        if ((down || mid) && !up && Utils.keyPressed(playerControl.Player.MoveForward, pit, isClone) && !playerMovement.OnSlope())
        {
            if (lowClimbEnabled)
            {
                climbing = true;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
            }

            if (mid)
            {
                climbing = true;
                highClimb = true;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
            }
        }

        if (climbing)
        {
            player.position = new Vector3(player.position.x, player.position.y + climbSpeed * Time.fixedDeltaTime, player.position.z);
            if (!down && !mid && !up)
            {
                climbing = false;
                highClimb = false;
                rb.useGravity = true;
                rb.AddForce(Vector3.up * 4, ForceMode.Impulse);
            }
        }
    }
}
