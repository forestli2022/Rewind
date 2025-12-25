using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechNavigation : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    Vector3 destination;
    [Header("References")]
    public MechView mechView;
    [SerializeField] private Transform turret;
    [SerializeField] private float turretTurningTime;
    [SerializeField] private float turretRotateRange;

    [Header("Ranging")]
    MechShoot mechShoot;
    [SerializeField] private float comfortRange;
    [SerializeField] private Transform bulletSpawnPoint;
    [HideInInspector] public GameObject targetPlayer;
    [SerializeField]
    private LayerMask excludeEnemy;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.enabled = false;
        mechShoot = GetComponent<MechShoot>();
        targetPlayer = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetPlayer == null)
        {
            targetPlayer = GameObject.Find("Player");
        }

        if (agent.isActiveAndEnabled)
        {
            // set destination
            destination = targetPlayer.transform.position;
            agent.SetDestination(destination);
        }
        if (mechShoot.alert)
        {
            // turn turret
            Vector3 dir = (targetPlayer.transform.position - turret.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            float rotateX = lookRotation.eulerAngles.x;
            float rotateY = lookRotation.eulerAngles.y;
            float rotateZ = lookRotation.eulerAngles.z;
            if (dir.y > 0)
            {
                rotateX = Mathf.Clamp(rotateX - 360, -turretRotateRange, turretRotateRange);
            }
            else
            {
                rotateX = Mathf.Clamp(rotateX, -turretRotateRange, turretRotateRange);
            }
            Quaternion newRotation = Quaternion.Euler(new Vector3(rotateX, rotateY, rotateZ));
            turret.rotation = Quaternion.RotateTowards(turret.rotation, newRotation, turretTurningTime * Time.fixedDeltaTime);

            // check comfort shooting zone
            if (Vector3.Distance(targetPlayer.transform.position, bulletSpawnPoint.position) <= comfortRange)
            {
                Ray ray = new Ray(bulletSpawnPoint.position, targetPlayer.transform.position - transform.position);
                if ((Physics.Raycast(ray, out RaycastHit hit, comfortRange, excludeEnemy) && hit.transform.gameObject.layer == 6) || Vector3.Distance(targetPlayer.transform.position, bulletSpawnPoint.position) <= 2)
                {
                    // if player within shooting range and can be detected, stop moving;
                    agent.enabled = false;
                }
                else
                {
                    agent.enabled = true;
                }
            }
            else
            {
                agent.enabled = true;
            }
        }
        else
        {
            agent.enabled = false;
        }
    }
}
