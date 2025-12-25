using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.VFX;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    [SerializeField] private GameObject katana;
    private PlayerMovement pm;
    private PlayerClimb pc;
    private PlayerWallRun wr;
    private PlayerFinisher pf;
    private Rigidbody rb;
    private TimeControlVFX slashTimeControl;
    private PlayerControl playerControl;

    private float velZ;
    private float velX;

    private float jumpTimer;

    public bool isRunning;

    [HideInInspector] public bool sprinting;

    // clone modification
    private bool isClone;
    private PITInteractions pit;
    private ClonedPlayer cp;

    [Header("Pistol Related")]
    [SerializeField] private GameObject pistol;
    [SerializeField] private Transform sprintPistolTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private float pistolMovingTime;
    [SerializeField] private MultiAimConstraint weaponAimRig1;
    [SerializeField] private TwoBoneIKConstraint rightHandIK;

    [Header("Rigging")]
    [SerializeField] private Transform handTarget;
    [SerializeField] private Rig rightHandFingerIK;
    [HideInInspector] public bool aiming;
    [HideInInspector] public bool canShoot;
    [Header("Reload")]
    [SerializeField] private PistolBehaviour pistolBehaviour;
    [SerializeField] private Transform reloadPistolTransform;

    [Header("Sword Trail")]
    private GameObject swordTrail;

    // Start is called before the first frame update
    void Start()
    {
        isClone = gameObject.tag == "Clone";
        if (isClone)
        {
            cp = GetComponent<ClonedPlayer>();
        }
        playerControl = GetComponent<PlayerInput>().playerControl;

        pm = GetComponent<PlayerMovement>();
        wr = GetComponent<PlayerWallRun>();
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerClimb>();
        pf = GetComponent<PlayerFinisher>();
        swordTrail = GameObject.Find("Environment/Player Related/Slash");
        slashTimeControl = swordTrail.GetComponent<TimeControlVFX>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isClone = gameObject.tag == "Clone";
        // pit for clone
        if (isClone)
        {
            pit = cp.pit;
        }

        if (Utils.keyPressed(playerControl.Player.Sprint, pit, isClone))
        {
            sprinting = true;
        }
        else
        {
            sprinting = false;
        }

        // z direction
        if (Utils.keyPressed(playerControl.Player.MoveForward, pit, isClone))
        {
            if (velZ < 0)
            {
                velZ += 2 * Time.fixedDeltaTime;
            }
            if (velZ <= 0.5f || (Utils.keyPressed(playerControl.Player.Sprint, pit, isClone) && velZ <= 1))
            {
                velZ += Time.fixedDeltaTime;
            }
            else if (velZ > 0)
            {
                velZ -= Time.fixedDeltaTime;
            }

        }
        else if (Utils.keyPressed(playerControl.Player.MoveBackward, pit, isClone))
        {
            if (velZ > 0)
            {
                velZ -= 2 * Time.fixedDeltaTime;
            }
            if (velZ >= -0.5f || (Utils.keyPressed(playerControl.Player.Sprint, pit, isClone) && velZ >= -1))
            {
                velZ -= Time.fixedDeltaTime;
            }
        }
        else
        {
            velZ = Mathf.Lerp(velZ, 0, 20 * Time.fixedDeltaTime);
        }

        // x direction
        if (Utils.keyPressed(playerControl.Player.MoveRight, pit, isClone))
        {
            if (velX < 0)
            {
                velX += 2 * Time.fixedDeltaTime;
            }
            if (velX <= 0.5f || (Utils.keyPressed(playerControl.Player.Sprint, pit, isClone) && velX <= 1))
            {
                velX += Time.fixedDeltaTime;
            }

        }
        else if (Utils.keyPressed(playerControl.Player.MoveLeft, pit, isClone))
        {
            if (velX > 0)
            {
                velX -= 2 * Time.fixedDeltaTime;
            }
            if (velX >= -0.5f || (Utils.keyPressed(playerControl.Player.Sprint, pit, isClone) && velX >= -1))
            {
                velX -= Time.fixedDeltaTime;
            }
        }
        else
        {
            velX = Mathf.Lerp(velX, 0, 20 * Time.fixedDeltaTime);
        }

        if ((Utils.keyPressed(playerControl.Player.MoveForward, pit, isClone) || Utils.keyPressed(playerControl.Player.MoveLeft, pit, isClone) ||
        Utils.keyPressed(playerControl.Player.MoveBackward, pit, isClone) || Utils.keyPressed(playerControl.Player.MoveRight, pit, isClone) && rb.velocity.magnitude > 2f && pm.grounded))
        {
            // running
            animator.SetBool("isRunning", true);
            isRunning = true;
        }
        else
        {
            // idle
            animator.SetBool("isRunning", false);
            isRunning = false;
        }

        animator.SetFloat("velocityZ", velZ);
        animator.SetFloat("velocityX", velX);


        // jumping
        if (Utils.keyPressed(playerControl.Player.Jump, pit, isClone) && (pm.grounded || wr.isWallRunning))
        {
            animator.SetBool("jumped", true);
            jumpTimer = 0f;
        }

        if (animator.GetBool("jumped"))
        {
            jumpTimer += Time.fixedDeltaTime;
        }

        if (pm.grounded && jumpTimer > 0.3f)
        {
            animator.SetBool("jumped", false);
        }

        // falling
        if (!pm.grounded && !animator.GetBool("jumped"))
        {
            animator.SetBool("falling", true);
        }
        else if (pm.grounded)
        {
            animator.SetBool("falling", false);
        }
        animator.SetFloat("jumpedTime", jumpTimer);

        // wall running
        if (wr.isWallRunning)
        {
            animator.SetBool("wallLeft", wr.wallLeft);
            animator.SetBool("wallRight", wr.wallRight);
            animator.SetBool("falling", false);
        }
        else
        {
            animator.SetBool("wallLeft", false);
            animator.SetBool("wallRight", false);
        }

        // sliding
        if (pm.crouched && pm.grounded)
        {
            animator.SetBool("sliding", true);
        }
        else
        {
            animator.SetBool("sliding", false);
        }

        // climbing
        animator.SetBool("climbing", pc.highClimb);



        // weapon positioning
        rightHandFingerIK.weight = 1;
        if (!wr.isWallRunning && !pc.highClimb && !animator.GetBool("sliding") && !pistolBehaviour.reloading && pf.finisherType == 0)
        {
            rightHandIK.weight = 1;
            weaponAimRig1.weight = Mathf.Lerp(weaponAimRig1.weight, 1, pistolMovingTime * Time.fixedDeltaTime);
            pistol.transform.position = Vector3.Lerp(pistol.transform.position, sprintPistolTransform.position, pistolMovingTime * Time.fixedDeltaTime);
            pistol.transform.right = transform.Find("Armature").right;
        }
        else if (wr.isWallRunning || pc.highClimb || animator.GetBool("sliding") || pf.finisherType > 0)
        { // performing other tasks
            weaponAimRig1.weight = 0;
            rightHandIK.weight = 0;
            pistol.transform.position = rightHandTransform.position;
            pistol.transform.rotation = rightHandTransform.rotation;
        }
        else
        {
            // reloading
            rightHandIK.weight = 1;
            if (pistolBehaviour.currentReloadTime < pistolBehaviour.reloadTime - 0.3f)
            {
                weaponAimRig1.weight = 0;
                pistol.transform.rotation = Quaternion.Lerp(pistol.transform.rotation, reloadPistolTransform.rotation, pistolMovingTime * Time.fixedDeltaTime);
                pistol.transform.position = Vector3.Lerp(pistol.transform.position, reloadPistolTransform.position, pistolMovingTime * Time.fixedDeltaTime);
            }
            else
            {
                weaponAimRig1.weight = Mathf.Lerp(weaponAimRig1.weight, 1, pistolMovingTime * Time.fixedDeltaTime);
                pistol.transform.position = Vector3.Lerp(pistol.transform.position, sprintPistolTransform.position, pistolMovingTime * Time.fixedDeltaTime);
            }
        }

        if (!pistol.activeInHierarchy && pf.finisherType == 0)
        {
            rightHandIK.weight = 0;
            rightHandFingerIK.weight = 0;
        }

        // weapon aiming;
        if (pistol.activeInHierarchy)
        {
            if (animator.GetBool("sliding") || animator.GetBool("climbing") || wr.isWallRunning || pistolBehaviour.reloading)
            {
                canShoot = false;
                aiming = false;
            }
            else if (animator.GetBool("jumped") || (animator.GetBool("isRunning") && sprinting))
            {
                aiming = false;
                canShoot = true;
            }
            else
            {
                aiming = true;
                canShoot = true;
            }
        }
        else
        {
            canShoot = false;
            aiming = false;
        }

        // finisher
        if (pf.finisherType > 0)
        {
            if (pistol.activeInHierarchy)
            {
                pistol.SetActive(false);
            }
            if (!katana.activeInHierarchy)
            {
                katana.SetActive(true);
            }
            animator.SetInteger("finisherType", pf.finisherType);

        }
        else
        {
            if (pistolBehaviour.ownPistol && !pistol.activeInHierarchy)
            {
                pistol.SetActive(true);
            }
            if (pf.finisherType == 0 && katana.activeInHierarchy)
            {
                katana.SetActive(false);
            }
            animator.SetInteger("finisherType", 0);
        }

    }

    // message from animator
    public void RenderSwordTrail()
    {
        slashTimeControl.VFXIsPlaying();
        swordTrail.transform.position = katana.transform.position;
        swordTrail.transform.rotation = katana.transform.rotation;
        if (pf.finisherType != 1)
        {
            swordTrail.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            swordTrail.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        swordTrail.GetComponent<VisualEffect>().Play();
    }
}
