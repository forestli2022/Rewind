
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.VFX;

public class ClonedPlayer : MonoBehaviour
{
    // references
    private TimeControlPlayer tcp;
    private Transform body;
    private Transform cam;
    private PlayerControl playerControl;
    private GameObject pistolPickup;

    // rewinding
    private Timer timer;
    private float fastForwardScale;
    bool fastForwardAlready = false;
    private bool rewinding = true;
    public List<PITInteractions> pitList;
    public PITInteractions pit;
    private List<PITInteractions> temp;
    private bool destroyedPlayer = false;

    // audio
    AudioSource[] audio;

    void Start()
    {
        // get main player time control player
        tcp = GameObject.Find("Player").GetComponent<TimeControlPlayer>();
        timer = GameObject.Find("Time").GetComponent<Timer>();

        pistolPickup = GameObject.Find("Environment/Player Related/Pistol Pickup/Holder");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // assign player control script
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;

        body = transform.Find("Armature");
        body.rotation = transform.Find("Orientation").rotation;


        // initialize when starts in the past
        if (!timer.rewinding && rewinding)
        {
            pastInit();
        }

        // still rewinding
        if (rewinding)
        {
            if (temp == null)
            {
                temp = new List<PITInteractions>(GameObject.Find("Player").GetComponent<TimeControlPlayer>().inputsList);
                pitList = new List<PITInteractions>();
            }
            pitList.Insert(0, temp[temp.Count - pitList.Count - 1]);
        }

        // in past, but not rewinding
        if (!rewinding && pitList.Count > 0)
        {
            pit = pitList[0];
            pitList.RemoveAt(0);
            transform.Find("Orientation").rotation = pit.orientation;
            cam = transform.Find("Camera Holder");
            cam.rotation = pit.camRot;

            // fix hand IK bug
            if (GetComponent<PistolBehaviour>().ownPistol)
            {
                Transform rightHandTarget = transform.Find("Armature/Armature@T-Pose/PISTOL/right_hand_target");
                Transform currentMiddleFinger = transform.Find("Armature/Armature@T-Pose/Skeleton/Hips/Spine/Chest/UpperChest/Right_Shoulder/Right_UpperArm/Right_LowerArm/Right_Hand/Right_MiddleProximal");
                Transform desiredLocation = transform.Find("Armature/Armature@T-Pose/PISTOL/Middle finger target");
                Utils.IKBugFix(rightHandTarget, currentMiddleFinger, desiredLocation);
            }
        }

        // destroy the player when getting back to the current
        if (pitList.Count <= 0 && !destroyedPlayer)
        {
            // end rewinding
            // if player is still in finisher animation, exit cleanly
            GameObject player = GameObject.Find("Player");
            PlayerFinisher pf = player.GetComponent<PlayerFinisher>();
            if (pf.finisherType != 0)
            {
                pf.targetEnemy.GetComponent<MechCombatState>().dead = false;
                pf.targetEnemy.GetComponent<MechNavigation>().enabled = true;
                pf.targetEnemy.GetComponent<MechShoot>().enabled = true;
                pf.targetEnemy.GetComponent<NavMeshAgent>().enabled = true;
                pf.targetEnemy.transform.Find("Holder/body").GetComponent<BoxCollider>().enabled = true;
                pf.targetEnemy.transform.Find("Holder/turret").GetComponent<BoxCollider>().enabled = true;
                pf.targetEnemy.transform.Find("Holder/Armature/Hip/Turret").GetComponent<MechRoutine>().enabled = true;
                pf.targetEnemy.transform.Find("Holder/Armature/Hip/Turret/view zone").GetComponent<MechView>().enabled = true;
            }

            // if the destroyed player carries pistol, produce the pistol pickup
            if (player.GetComponent<PistolBehaviour>().ownPistol)
            {
                pistolPickup.SetActive(true);
                pistolPickup.transform.position = player.transform.position;
            }

            Destroy(player);
            PlayerLook pl = gameObject.GetComponent<PlayerLook>();
            pl.isClone = true;
            float xRotTemp = transform.Find("Camera Holder").eulerAngles.x;
            while (xRotTemp > pl.xLim || xRotTemp < -pl.xLim)
            {
                if (xRotTemp > pl.xLim)
                {
                    xRotTemp -= 360;
                }
                else
                {
                    xRotTemp += 360;
                }
            }


            pl.xRotation = xRotTemp;
            pl.yRotation = transform.Find("Orientation").eulerAngles.y;

            transform.Find("Camera Holder").gameObject.SetActive(true);
            destroyedPlayer = true;
        }

        if (pitList.Count <= 0)
        {
            gameObject.name = "Player";
            tag = "Player";
            fastForwardAlready = false;

            // enable player look
            GetComponent<PlayerLook>().enabled = true;
            GetComponent<PlayerLook>().yRotation = body.rotation.eulerAngles.y;
            GetComponent<ClonedPlayer>().enabled = false;  // disable self

            // enable UI
            transform.Find("in_game_UI").gameObject.SetActive(true);

            GetComponent<TimeControlPlayer>().enabled = true;
        }
    }

    private void pastInit()
    {
        rewinding = false;
        // enable scripts
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerMovement>().speed = pitList[0].speed;
        GetComponent<PlayerWallRun>().enabled = true;
        GetComponent<PlayerClimb>().enabled = true;
        GetComponent<PlayerAnimation>().enabled = true;
        GetComponent<PistolBehaviour>().enabled = true;
        GetComponent<PlayerCombatState>().enabled = true;
        GetComponent<PlayerFinisher>().enabled = true;
        GetComponent<CameraFollow>().enabled = true;
        GetComponent<PickUpCube>().enabled = true;

        // enable animator
        transform.Find("Armature/Armature@T-Pose").gameObject.GetComponent<Animator>().enabled = true;
        fastForwardScale = GameObject.Find("Time").GetComponent<Timer>().fastForwardScale;
        transform.Find("Armature/Armature@T-Pose/rig_layers_aiming").gameObject.GetComponent<Rig>().weight = 1;

        // audio
        audio = GetComponents<AudioSource>();

        // enable all mesh renderers and enable orb
        VisualEffect orb = transform.Find("Orb").GetComponent<VisualEffect>();
        orb.SetFloat("ParticleSpawnRate", 0);
        orb.SetFloat("TrailSpawnRate", 0);
        foreach (MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = true;
        }
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
    }
}
