using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechView : MonoBehaviour
{
    [SerializeField] private MechNavigation mn;
    [SerializeField] private NavMeshAgent agent;
    [Header("Light ray setting")]
    public Light light;
    public Color normalColor;
    public Color battleColor;
    [SerializeField] private Transform turret;
    [SerializeField] private float rayDist;
    [SerializeField] private MechShoot mechshoot;
    // time counter when the target is lost
    public float timeTargetLost = 3f;
    [HideInInspector] public float timeNotSeen;
    [SerializeField] private Transform mechTransform;
    [HideInInspector] public bool seenThisFrame;

    [Header("View")]
    [SerializeField] LayerMask excludeEnemy;


    // Start is called before the first frame update
    void Start()
    {
        light.color = normalColor;
        timeNotSeen = timeTargetLost;  // in the beginning, the mech does not have a target, therefore not alert
    }

    void FixedUpdate()
    {
        // increment time not seen player
        timeNotSeen += Time.fixedDeltaTime;
        if (timeNotSeen >= timeTargetLost)
        {
            mechshoot.alert = false;
            light.color = normalColor;
        }
    }

    void OnTriggerStay(Collider col)
    {
        seenThisFrame = false;
        if (col.gameObject.layer == 6 && this.enabled)
        {
            Vector3 collisionPoint = col.ClosestPoint(transform.position);
            Vector3 dir = Vector3.Normalize(collisionPoint - turret.position);
            Ray ray = new Ray(turret.position, dir);
            Debug.DrawRay(turret.position, dir * rayDist);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDist, excludeEnemy) && hit.transform.gameObject.layer == 6)
            {
                setAlert(hit.transform.gameObject);
            }
        }
    }

    public void setAlert(GameObject target)
    {
        timeNotSeen = 0f;
        mechshoot.alert = true;
        seenThisFrame = true;
        if (target.tag == "Clone")
        {
            mn.targetPlayer = GameObject.Find("Player(Clone)");
        }
        else
        {
            mn.targetPlayer = GameObject.Find("Player");
        }
        light.color = battleColor;
        agent.enabled = true;
    }
}
