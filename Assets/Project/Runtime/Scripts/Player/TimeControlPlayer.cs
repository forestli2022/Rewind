using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class TimeControlPlayer : TimeControlParent
{
    // game objects / references
    private Transform orientation;
    private Transform orientationC;
    private GameObject clone;
    public Camera cam;
    private GameObject camHolder;
    private GameObject pistol;
    private GameObject katana;

    private Rigidbody rb;
    private Rigidbody rbC;
    private RigidbodyState currentRigidbodyState;


    // rewinding data
    private int cnt = 0;
    public bool inPast;
    private bool rewindPressed;

    // scripts
    private PlayerLook pl;
    private PlayerCombatState pcs;
    private PistolBehaviour pb;
    private PlayerFinisher pf;
    private PickUpCube puc;
    private PlayerControl playerControl;

    // keycodes to be checked.
    public PlayerInput pi;
    // Physical Player list
    [HideInInspector] public List<PITPlayer> pitList = new List<PITPlayer>();

    // InputsPITList (needed to be accessed by clones)
    [HideInInspector] public List<PITInteractions> inputsList = new List<PITInteractions>();
    // animation
    private float animationTime;
    private string animationName;

    // sound
    private AudioSource[] audio;


    protected override void StartInit()
    {
        // script referencing
        pl = GetComponent<PlayerLook>();
        pcs = GetComponent<PlayerCombatState>();
        pb = GetComponent<PistolBehaviour>();
        pf = GetComponent<PlayerFinisher>();
        puc = GetComponent<PickUpCube>();
        // others
        rb = GetComponent<Rigidbody>();

        orientation = transform.Find("Orientation");
        audio = GetComponents<AudioSource>();
        maxRecordingTime = time.GetComponent<Timer>().maximumRecordingTime;
        camHolder = transform.Find("Camera Holder").gameObject;
        pistol = transform.Find("Armature/Armature@T-Pose/PISTOL").gameObject;
        katana = transform.Find("Armature/Armature@T-Pose/Skeleton/Hips/Spine/Chest/UpperChest/Right_Shoulder/Right_UpperArm/Right_LowerArm/Right_Hand/Katana").gameObject;
    }

    protected override void FixedUpdate()
    {
        playerControl = GetComponent<PlayerInput>().playerControl;

        // set rewinding state
        if (!rewindPressed && playerControl.Player.Rewind.IsPressed())
        {
            if (!timer.rewinding && timer.cd == 0)
            {
                timer.rewinding = true;
            }
            else
            {
                timer.rewinding = false;
            }
        }

        // if rewind to limit, stop rewinding
        if (pitList.Count == 0)
        {
            timer.rewinding = false;
        }

        if (timer.rewinding)
        {
            Rewinding();
        }
        else
        {
            NotRewinding();
        }

        rewindPressed = playerControl.Player.Rewind.IsPressed();
    }

    protected override void Rewinding()
    {
        // setting state for rewinding in the first frame
        if (!inPast)
        {
            rewindingInit();
        }

        // set state to clone
        SetState(pitList[pitList.Count - 1]);

        pitList.RemoveAt(pitList.Count - 1);
        cnt++;
        firstFrameAfter = true;
        inPast = true;
    }

    protected override void NotRewinding()
    {
        // set clone state
        if (clone != null && rbC.isKinematic != false)
        {
            rbC.isKinematic = false;
            // enable collider
            clone.transform.Find("Armature").gameObject.GetComponent<CapsuleCollider>().enabled = true;
            clone.transform.Find("Armature/Armature@T-Pose/Skeleton/Hips/Spine/Chest/UpperChest/Neck/Head").gameObject.GetComponent<SphereCollider>().enabled = true;
            // enable scripts
            pf.enabled = true;
            puc.enabled = true;
        }

        // clamp maximum pits number
        if (pitList.Count > maxRecordingTime / Time.fixedDeltaTime)
        {
            // remove items exceeding recording limit
            pitList.RemoveAt(0);
            inputsList.RemoveAt(0);
        }

        // get state
        GetState();

        // if still in past
        if (inPast)
        {
            cnt--;
            // play the rewinding audio when going back to current
            if (cnt == (int)(audio[1].clip.length / Time.fixedDeltaTime))
            {
                audio[1].Play(0);
            }
        }

        // if ready to go back to current
        if (cnt < 1)
        {
            inPast = false;
        }

        // immediately after rewind
        if (firstFrameAfter)
        {
            // add velocity for the player clone
            currentRigidbodyState.SetRigidbody(rbC);
            // set player finisher animation
            Animator cloneAnimator = clone.transform.Find("Armature/Armature@T-Pose").GetComponent<Animator>();
            cloneAnimator.Play("Base Layer." + animationName, 0, animationTime);
            // enable

            firstFrameAfter = false;
            pitList.Clear();
            inputsList.Clear();
        }
    }



    private void rewindingInit()
    {
        // disable player script
        pf.enabled = false;
        puc.enabled = false;


        clone = Instantiate(gameObject, transform.position, transform.rotation);

        // disable scripts
        clone.GetComponent<TimeControlPlayer>().enabled = false;
        clone.GetComponent<PlayerMovement>().enabled = false;
        clone.GetComponent<PlayerLook>().enabled = false;
        clone.GetComponent<CameraFollow>().enabled = false;
        clone.GetComponent<PlayerWallRun>().enabled = false;
        clone.GetComponent<PlayerClimb>().enabled = false;
        clone.GetComponent<PlayerAnimation>().enabled = false;
        clone.GetComponent<PistolBehaviour>().enabled = false;
        clone.GetComponent<PlayerCombatState>().enabled = false;
        clone.GetComponent<PlayerFinisher>().enabled = false;
        clone.GetComponent<PickUpCube>().enabled = false;


        // disable animator
        clone.transform.Find("Armature/Armature@T-Pose").gameObject.GetComponent<Animator>().enabled = false;
        clone.transform.Find("Armature/Armature@T-Pose/rig_layers_aiming").gameObject.GetComponent<Rig>().weight = 0;


        // get rigid body and set it to kinematic
        rbC = clone.GetComponent<Rigidbody>();
        rbC.isKinematic = true;

        // disable collider
        clone.transform.Find("Armature").gameObject.GetComponent<CapsuleCollider>().enabled = false;
        clone.transform.Find("Armature/Armature@T-Pose/Skeleton/Hips/Spine/Chest/UpperChest/Neck/Head").gameObject.GetComponent<SphereCollider>().enabled = false;

        // get orientation
        orientationC = clone.transform.Find("Orientation");

        // enable clone script
        clone.GetComponent<ClonedPlayer>().enabled = true;

        // deactive camera object
        GameObject cloneCam = clone.transform.Find("Camera Holder").gameObject;
        cloneCam.SetActive(false);

        // set tag to Clone
        clone.tag = "Clone";

        // play audio drop
        audio[0].Play(0);

        // disable pistol gameobject
        transform.Find("Armature/Armature@T-Pose/PISTOL").gameObject.SetActive(false);
        GetComponent<PistolBehaviour>().ownPistol = false;

        // disable clone UI
        clone.transform.Find("in_game_UI").gameObject.SetActive(false);

        // disable all mesh renderers and enable orb
        VisualEffect orb = clone.transform.Find("Orb").GetComponent<VisualEffect>();
        orb.SetFloat("ParticleSpawnRate", 500000);
        orb.SetFloat("TrailSpawnRate", 30);

        foreach (MeshRenderer mr in clone.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }
        clone.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
    }


    private void SetState(PITPlayer pit)  // setting the state of cloned player at that point in time
    {
        // set state
        if (clone != null)
        {
            // set combat data
            PlayerCombatState cloneCombatState = clone.GetComponent<PlayerCombatState>();
            PistolBehaviour clonePb = clone.GetComponent<PistolBehaviour>();
            PlayerFinisher clonePf = clone.GetComponent<PlayerFinisher>();
            PickUpCube clonePuc = clone.GetComponent<PickUpCube>();

            pit.SetState(out currentRigidbodyState, out cloneCombatState.health, out cloneCombatState.damageTimer, out bool pistolActive, out pb.bulletNum, out clonePb.reloading, out clonePb.currentReloadTime, out bool katanaActive, out clonePf.finisherType, out clonePf.finisherTime, out clonePf.targetEnemy, out animationName, out animationTime, out clonePuc.hasCube, out clonePuc.currentCube);

            rbC.position = currentRigidbodyState.position;
            orientationC.rotation = currentRigidbodyState.rotation;

            clone.transform.Find("Armature/Armature@T-Pose/PISTOL").gameObject.SetActive(pistolActive);
            clone.GetComponent<PistolBehaviour>().ownPistol = pistolActive;
            clone.transform.Find("Armature/Armature@T-Pose/Skeleton/Hips/Spine/Chest/UpperChest/Right_Shoulder/Right_UpperArm/Right_LowerArm/Right_Hand/Katana").gameObject.SetActive(katanaActive);
        }
    }

    private void GetState()
    {
        // player combat data
        pitList.Add(new PITPlayer(new RigidbodyState(rb), pcs.health, pcs.damageTimer, pistol.activeInHierarchy, pcs.pistolBulletNum, pcs.reloading, pcs.currentReloadTime, katana.activeInHierarchy, pcs.finisherType, pcs.finisherTime, pcs.finisherTargetEnemy, pcs.animationName, pcs.animationTime, puc.hasCube, puc.currentCube));

        // interactions
        // fetching key presses
        List<string> inputActions = new List<string>();
        foreach (InputAction action in pi.buttons)
        {
            if (action.IsPressed())
            {
                inputActions.Add(action.name);
            }
        }

        inputsList.Add(new PITInteractions(inputActions, orientation.rotation, GetComponent<PlayerMovement>().speed, camHolder.transform.rotation));


    }

    // message from checkpoint
    public void ClearList()
    {
        pitList.Clear();
    }
}
