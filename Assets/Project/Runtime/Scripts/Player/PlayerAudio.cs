using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // references
    private AudioSource[] audio;
    private PlayerMovement pm;
    private PlayerWallRun wr;
    private PlayerClimb pc;
    private PlayerAnimation pa;
    private Rigidbody rb;

    [SerializeField] private float velocityThreshold;
    [Range(0, 1)][SerializeField] private float windScale;

    private int wallRunAudio;

    private bool wasGrounded;
    private bool wasWallRunning;
    private bool wasCrouched;

    // rewinding
    private PlayerControl playerControl;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = transform.parent.parent.gameObject;
        audio = player.GetComponents<AudioSource>();
        pm = player.GetComponent<PlayerMovement>();
        wr = player.GetComponent<PlayerWallRun>();
        pc = player.GetComponent<PlayerClimb>();
        rb = player.GetComponent<Rigidbody>();
        pa = player.GetComponent<PlayerAnimation>();
    }



    void FixedUpdate()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;

        if (rb.velocity.magnitude > velocityThreshold)
        {
            audio[3].volume = Mathf.Lerp(audio[3].volume, (rb.velocity.magnitude - velocityThreshold) * windScale, 10 * Time.fixedDeltaTime);
            audio[3].volume = Mathf.Clamp(audio[3].volume, 0, 1);
        }
        else
        {
            audio[3].volume = Mathf.Lerp(audio[3].volume, 0, 10 * Time.fixedDeltaTime);
        }

        if (pm.grounded && !wasGrounded && rb.velocity.y < -3)
        {
            audio[6].pitch = Random.Range(0.9f, 1.1f);
            audio[6].Play();
        }
        wasGrounded = pm.grounded;

        if (wr.isWallRunning && !wasWallRunning)
        {
            audio[7].pitch = Random.Range(0.9f, 1.1f);
            audio[7].Play();
        }
        wasWallRunning = wr.isWallRunning;

        if (pm.crouched)
        {
            if (!wasCrouched)
            {
                audio[8].volume = 1;
                audio[8].Play();
            }
            else
            {
                audio[8].volume -= 0.5f * Time.fixedDeltaTime;
            }
        }
        else
        {
            audio[8].Stop();
        }
        wasCrouched = pm.crouched;

        if (pm.grounded && playerControl.Player.Jump.IsPressed() && !audio[9].isPlaying)
        {
            audio[9].Play();
        }

        if (pc.climbing && !audio[10].isPlaying)
        {
            audio[10].Play();
        }
    }




    // animation events functions
    private void Step()
    {
        if (!audio[2].isPlaying && pm.grounded)
        {
            audio[2].pitch = Random.Range(1.6f, 2f);
            audio[2].Play();
        }
    }

    private void WallRun()
    {
        if (wr.isWallRunning)
        {
            if (wallRunAudio == 1)
            {
                audio[5].pitch = Random.Range(0.5f, 0.7f);
                audio[5].Play();
                wallRunAudio = 2;
            }
            else
            {
                audio[4].pitch = Random.Range(0.5f, 0.7f);
                audio[4].Play();
            }
        }
        else
        {
            audio[5].Stop();
            audio[4].Stop();
        }
    }

    private void SwordSlash()
    {
        audio[11].Play();
    }

    private void SwordSpin()
    {
        audio[12].Play();
    }

    private void SwordJump()
    {
        audio[13].Play();
    }

    private void SwordTrail()
    {
        pa.RenderSwordTrail();
    }

    public void PlayDamagedAudio()
    {
        audio[14].Play();
    }

    public void PlayPickupAudio()
    {
        audio[15].Play();
    }
}
