using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerFinisher : MonoBehaviour
{
    [Header("Variables For Finishers")]
    // three different finishers, low speed, high speed and mid air
    [SerializeField] private float walkingFinisherRange;
    [SerializeField] private float sprintingFinisherRange;
    [SerializeField] private float midAirFinisherRange;
    [SerializeField] private float walkingFinisherTime;
    [SerializeField] private float sprintingFinisherTime;
    [SerializeField] private float midAirFinisherTime;
    [SerializeField] private float walkingFinisherSpeed;
    [SerializeField] private float sprintingFinisherSpeed;
    [SerializeField] private float midAirFinisherSpeed;
    [SerializeField] private float midAirFinisherZOffSet;
    [SerializeField] private float sprintingFinisherZOffSet;
    [SerializeField] private float sprintingFinisherXOffSet;
    [HideInInspector] public int finisherType = 0;   // 0 = none, 1 = low speed, 2 = high speed, 3 = mid air
    [HideInInspector] public bool canExecute;
    [HideInInspector] public float finisherTime;
    private Vector3 oldPos;
    [HideInInspector] public GameObject targetEnemy;

    [Header("References")]
    [SerializeField] private Transform aim;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private PlayerLook pl;
    [SerializeField] private PickUpCube puc;
    [SerializeField] private Transform orientation;


    // clone modification
    private bool isClone;
    private PITInteractions pit;
    private ClonedPlayer cp;
    private PlayerControl playerControl;
    // Start is called before the first frame update
    void Start()
    {
        isClone = gameObject.tag == "Clone";
        if (isClone)
        {
            cp = GetComponent<ClonedPlayer>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerControl = GetComponent<PlayerInput>().playerControl;
        isClone = gameObject.tag == "Clone";
        // pit for clone
        if (isClone)
        {
            pit = cp.pit;
        }

        Finisher();
    }

    private void Finisher()
    {
        // if player is tring to pickup a cube or place a cube, it has a higher priority than this
        if (puc.canPickUp || puc.hasCube)
        {
            return;
        }

        if (finisherType == 0)
        {
            targetEnemy = FindTargetEnemy(Mathf.Max(sprintingFinisherRange, midAirFinisherRange));
        }
        float distance = targetEnemy != null ? Vector3.Distance(targetEnemy.transform.position, transform.position) : float.MaxValue;

        if (Utils.keyPressed(playerControl.Player.Sprint, pit, isClone) && pm.grounded && distance <= sprintingFinisherRange && distance > walkingFinisherRange)
        {
            // perform long range finisher check
            if (targetEnemy != null)
            {
                canExecute = true;
                if (Utils.keyPressed(playerControl.Player.MultiFunction, pit, isClone) && finisherType == 0)
                {
                    finisherType = 2;
                    finisherTime = 0;
                    oldPos = transform.position;
                }
            }
        }
        else if (pm.grounded && distance <= walkingFinisherRange)
        {
            // perform low range finisher check
            if (targetEnemy != null)
            {
                canExecute = true;
                if (Utils.keyPressed(playerControl.Player.MultiFunction, pit, isClone) && finisherType == 0)
                {
                    finisherType = 1;
                    finisherTime = 0;
                    oldPos = transform.position;
                }
            }
        }
        else if (!pm.grounded && distance <= midAirFinisherRange)
        {
            // perform midair finisher check
            if (targetEnemy != null)
            {
                canExecute = true;
                if (Utils.keyPressed(playerControl.Player.MultiFunction, pit, isClone) && finisherType == 0)
                {
                    finisherType = 3;
                    finisherTime = 0;
                    oldPos = transform.position;
                }
            }
        }
        else
        {
            canExecute = false;
        }

        // disabling all irrelevant scripts
        if (finisherType != 0)
        {
            finisherTime += Time.fixedDeltaTime;
            if (pm.enabled)
            {
                playerControl.Disable();
                pm.enabled = false;
                if (finisherType != 1)
                {
                    pl.freeToRotate = false;
                }
                rb.isKinematic = true;
            }
        }
        else
        {
            playerControl.Enable();
            rb.isKinematic = false;
            pm.enabled = true;
            pl.freeToRotate = true;
        }

        // execute translation
        Vector3 targetPos = Vector3.zero;
        if (finisherType != 0 && targetEnemy != null)
        {
            targetEnemy.SendMessage("Die");
            Vector3 diff = new Vector3(transform.position.x - targetEnemy.transform.position.x, 0, transform.position.z - targetEnemy.transform.position.z).normalized;
            targetPos = targetEnemy.transform.position;
            if (finisherType == 2)
            {
                targetPos += orientation.right * sprintingFinisherXOffSet;
                targetPos += diff * sprintingFinisherZOffSet;
            }
            else
            {
                targetPos += diff * midAirFinisherZOffSet;
            }
        }

        if (finisherType == 1)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, walkingFinisherSpeed * Time.fixedDeltaTime);
            if (finisherTime >= walkingFinisherTime)
            {
                finisherType = 0;
                finisherTime = 0;
                targetEnemy.SendMessage("StartCountDown");
            }
        }
        else if (finisherType == 2)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, sprintingFinisherSpeed * Time.fixedDeltaTime);
            if (finisherTime >= sprintingFinisherTime)
            {
                finisherType = 0;
                finisherTime = 0;
                targetEnemy.SendMessage("StartCountDown");
            }
        }
        else if (finisherType == 3)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, walkingFinisherSpeed * Time.fixedDeltaTime);
            if (finisherTime >= midAirFinisherTime)
            {
                finisherType = 0;
                finisherTime = 0;
                targetEnemy.SendMessage("StartCountDown");
            }
        }
    }

    private GameObject FindTargetEnemy(float range)
    {
        GameObject result = null;

        if (Physics.Raycast(transform.position, (aim.position - transform.position).normalized, out RaycastHit hit, range))
        {
            if (hit.collider.gameObject.layer == 7 && hit.collider.transform.parent.parent.gameObject.GetComponent<MechCombatState>().health <= 0)
            {
                result = hit.collider.transform.parent.parent.gameObject;
            }
        }
        return result;
    }
}
