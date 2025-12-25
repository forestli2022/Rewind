using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechIKFootSolver : MonoBehaviour
{
    [SerializeField] private float rayDist;
    [SerializeField] private LayerMask layermask;
    [SerializeField] private MechIKFootSolver otherFoot;
    [SerializeField] private Transform body;
    [SerializeField] private float stepDist;
    [SerializeField] private float stepLength;
    [SerializeField] private float stepHeight;
    [SerializeField] private float speed;
    [SerializeField] private float footSpacing;
    [SerializeField] private Vector3 offset;

    [HideInInspector]
    public Vector3 oldPos, currentPos, newPos;
    [HideInInspector]
    public Vector3 oldNorm, currentNorm, newNorm;
    [HideInInspector]
    public float lerp;
    public bool moved;

    // rotation
    private Vector3 oldBodyPos;

    // audio
    private AudioSource walkSound;


    // Start is called before the first frame update
    void Start()
    {
        currentPos = newPos = oldPos = transform.position;
        currentNorm = newNorm = oldNorm = transform.up;
        lerp = 1;
        moved = false;
        oldBodyPos = body.position;
        walkSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (body.position == oldBodyPos)
        {
            currentPos = newPos = oldPos = transform.position;
            currentNorm = newNorm = oldNorm = transform.up;
            lerp = 1;
            moved = false;


            Ray ray1 = new Ray(body.position + (body.right * footSpacing), Vector3.down);
            if (Physics.Raycast(ray1, out RaycastHit hit1, rayDist, layermask.value))
            {
                transform.position = Vector3.Lerp(transform.position, hit1.point, speed * Time.deltaTime);
                transform.up = Vector3.Lerp(transform.up, hit1.normal, speed * Time.deltaTime);
                transform.forward = body.forward;
                Debug.DrawRay(body.position + (body.right * footSpacing), Vector3.down * rayDist);
            }
            return;
        }
        transform.position = currentPos;
        transform.up = currentNorm;
        transform.forward = body.forward;


        // raycast downwards to find position to place feet
        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDist, layermask.value))
        {
            if (Vector3.Distance(oldPos, hit.point) > stepDist && !otherFoot.IsMoving() && lerp >= 1 && !moved)
            {
                moved = true;
                otherFoot.moved = false;
                lerp = 0;

                Vector3 posDiff = body.position - oldBodyPos;
                Vector3 dir = Vector3.Normalize(new Vector3(posDiff.x, 0, posDiff.z));

                newPos = hit.point + dir * stepLength;
                newNorm = hit.normal;

                // audio
                walkSound.Play();
            }
        }
        oldBodyPos = body.position;

        // if moving feet, it follows sin wave motion
        if (lerp < 1)
        {
            Vector3 tempPos = Vector3.Lerp(oldPos, newPos, lerp);
            tempPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPos = tempPos;
            currentNorm = Vector3.Lerp(oldNorm, newNorm, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            // cycle completed, make this point the oldPos and repeats
            oldPos = newPos;
            oldNorm = newNorm;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPos, 0.05f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}
