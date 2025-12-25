using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MechBulletBehaviour : MonoBehaviour
{
    [Header("General Settings")]
    public float speed;
    public float deSpawnTime = 5;
    public LayerMask enemyLayer;
    public float damage;

    [Header("Line Rendering")]
    [HideInInspector] public Vector3[] vertices = new Vector3[2];
    [HideInInspector] public float existingTime = 0;
    private LineRenderer lr;
    private Vector3 originalPos;
    public float lineRemainTime;

    [Header("Audio")]

    [SerializeField] private AudioClip[] bfbClips;
    [SerializeField] private AudioClip[] bhmCilps;
    public float bulletHeardRange;
    private bool soundPlayed = false;
    private Transform playerHead;
    [Header("Sparks")]
    public GameObject sparks;

    [Header("Rewinding management")]
    public BulletManager bulletManager;
    public Timer timer;
    private PlayerControl playerControl;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        timer = GameObject.Find("Time").GetComponent<Timer>();
    }

    void FixedUpdate()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;
        if (timer.rewinding)
        {  // REWINDING
            transform.position -= transform.forward * speed * Time.fixedDeltaTime;
            existingTime -= Time.fixedDeltaTime;
        }
        else
        { // NOT REWINDING
            playerHead = GameObject.Find("Player/Armature/Armature@T-Pose/Skeleton/Hips/Spine/Chest/UpperChest/Neck/Head").transform;

            // RayCast to see if bullet crosses any objects
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, speed * Time.fixedDeltaTime, enemyLayer))
            {
                if (hit.transform.gameObject.layer == 3 && Vector3.Distance(hit.point, playerHead.position) <= 3f)
                {
                    // bullet hit metal sound
                    AudioSource.PlayClipAtPoint(bhmCilps[Random.Range(0, 6)], hit.point, 0.2f);
                }

                // CREATE SPARKS
                if (hit.transform.gameObject.layer == 3)
                {
                    CreateSparks(hit);
                }

                // PLAYER HIT
                if (hit.transform.gameObject.layer == 6)
                {
                    hit.transform.root.gameObject.SendMessage("ReduceHealth", damage);
                }
                DestroyAndRecord();
            }
            else if (existingTime >= deSpawnTime)
            {
                DestroyAndRecord();
            }
            transform.position += transform.forward * speed * Time.fixedDeltaTime;


            // bullet passing by sound
            if (Vector3.Distance(playerHead.position, transform.position) <= bulletHeardRange && !soundPlayed)
            {
                AudioSource.PlayClipAtPoint(bfbClips[Random.Range(0, 9)], transform.position, 0.3f);
                soundPlayed = true;
            }

            // increment existing time
            existingTime += Time.fixedDeltaTime;
        }

        // if existing time == 0 (rewinded to spawn time, destroy game object)
        if (existingTime <= 0)
        {
            Destroy(gameObject);
        }

        RenderTrail();
    }

    private void DestroyAndRecord()
    {
        // record in bullet manager
        if (!timer.inPast)
        {
            bulletManager.destroyedBulletsList[bulletManager.destroyedBulletsList.Count - 1].Add(new DestroyedBullet(transform.position, transform.forward, existingTime));
        }
        Destroy(gameObject);
    }

    private void RenderTrail()
    {
        // render trail
        if (existingTime < lineRemainTime)
        {
            vertices[0] = transform.position - transform.forward * existingTime * speed; ;
        }
        else
        {
            vertices[0] = transform.position - transform.forward * lineRemainTime * speed;
        }
        vertices[1] = transform.position;

        lr.SetPositions(vertices);
    }

    private void CreateSparks(RaycastHit hit)
    {
        GameObject s = Instantiate(sparks, hit.point, Quaternion.identity);
        s.SetActive(true);
    }
}
