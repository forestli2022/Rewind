using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerBulletBehaviour : MonoBehaviour
{
    [Header("General")]
    public float speed;
    public float deSpawnTime = 5;
    [SerializeField] private GameObject player;

    [Header("Line Rendering")]
    [HideInInspector]
    public Vector3[] vertices = new Vector3[2];
    private Vector3 originalPos;
    [HideInInspector]
    public float existingTime = 0;
    LineRenderer lr;
    public float lineRemainTime;

    [Header("Audio")]
    [SerializeField] private AudioClip[] bhmCilps;
    [SerializeField] private AudioClip onHitSound;
    public float bulletHearingRange;
    bool soundPlayed = false;
    Transform playerHead;
    [Header("Sparks and onhit effect")]
    public GameObject sparks;
    public GameObject onhitEffect;

    [Header("Rewinding")]
    public BulletManager bulletManager;
    private Timer timer;
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
        {  // if rewinding
            transform.position -= transform.forward * speed * Time.fixedDeltaTime;
            existingTime -= Time.fixedDeltaTime;
        }
        else
        { // if not rewinding
            playerHead = GameObject.Find("Player/Armature/Armature@T-Pose/Skeleton/Hips/Spine/Chest/UpperChest/Neck/Head").transform;

            // RayCast to see if bullet crosses any objects
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, speed * Time.fixedDeltaTime))
            {
                if (Vector3.Distance(hit.point, playerHead.position) <= bulletHearingRange)
                {
                    // bullet hit metal sound
                    AudioSource.PlayClipAtPoint(bhmCilps[Random.Range(0, 6)], hit.point, 0.2f);
                }
                if (sparks != null && hit.collider.gameObject.layer != 7)  // hit obstacles
                {
                    CreateSparks(hit);
                }

                // check whether the hit target is an enemy
                if (hit.collider.gameObject.layer == 7)
                {
                    AudioSource.PlayClipAtPoint(onHitSound, playerHead.position, 0.3f);
                    CreateOnhitEffect(hit);
                    SendHitMessage(hit);
                }

                DestroyAndRecord();
            }
            else if (existingTime >= deSpawnTime)
            {
                DestroyAndRecord();
            }
            transform.position += transform.forward * speed * Time.fixedDeltaTime;

            // increment existing time
            existingTime += Time.fixedDeltaTime;
        }

        // if existing time == 0 (rewinded to spawn time, destroy game object)
        if (existingTime <= 0)
        {
            Destroy(gameObject);
        }

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

    void DestroyAndRecord()
    {
        // record in bullet manager
        if (!timer.inPast)
        {
            bulletManager.destroyedBulletsList[bulletManager.destroyedBulletsList.Count - 1].Add(new DestroyedBullet(transform.position, transform.forward, existingTime));
        }
        Destroy(gameObject);
    }

    private void CreateSparks(RaycastHit hit)
    {
        GameObject s = Instantiate(sparks, hit.point, Quaternion.identity);
        s.SetActive(true);
    }

    private void CreateOnhitEffect(RaycastHit hit)
    {
        GameObject onHit = Instantiate(onhitEffect, hit.point, Quaternion.identity);
        onHit.SetActive(true);
    }

    private void SendHitMessage(RaycastHit hit)
    {
        hit.collider.transform.parent.parent.SendMessage("BeingShot", player);
        player.SendMessage("BulletHitEnemy");
    }
}
