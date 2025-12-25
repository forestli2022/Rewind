using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    // nested list for all the bullets destroyed
    [HideInInspector]
    public List<List<DestroyedBullet>> destroyedBulletsList;
    private Timer timer;
    bool firstFrameAfter = false;
    public GameObject bullet;
    public GameObject sparks;
    public GameObject onHit;
    private PlayerControl playerControl;

    // Start is called before the first frame update
    void Start()
    {
        destroyedBulletsList = new List<List<DestroyedBullet>>();
        timer = GameObject.Find("Time").GetComponent<Timer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;
        if (timer.rewinding && destroyedBulletsList.Count > 0)
        {
            foreach (DestroyedBullet destroyedBullet in destroyedBulletsList[destroyedBulletsList.Count - 1])
            {
                GameObject b = Instantiate(bullet, destroyedBullet.position, Quaternion.identity);
                b.SetActive(true);
                // initializing bullet behaviour
                if (gameObject.layer == 7)
                {
                    MechBulletBehaviour mbb = b.GetComponent<MechBulletBehaviour>();
                    mbb.enabled = true;
                    mbb.sparks = sparks;

                    b.transform.forward = destroyedBullet.dir;
                    mbb.existingTime = destroyedBullet.existingTime;
                }
                else if (gameObject.layer == 6)
                {  // player
                    PlayerBulletBehaviour pbb = b.GetComponent<PlayerBulletBehaviour>();
                    pbb.enabled = true;
                    pbb.sparks = sparks;
                    pbb.onhitEffect = onHit;
                    b.transform.forward = destroyedBullet.dir;
                    pbb.existingTime = destroyedBullet.existingTime;
                }

            }
            destroyedBulletsList.RemoveAt(destroyedBulletsList.Count - 1);
            firstFrameAfter = true;
        }
        else
        {
            if (firstFrameAfter)
            {
                destroyedBulletsList.Clear();
                firstFrameAfter = false;
            }
            destroyedBulletsList.Add(new List<DestroyedBullet>());
            if (destroyedBulletsList.Count > timer.maximumRecordingTime / Time.fixedDeltaTime)
            {
                destroyedBulletsList.RemoveAt(0);
            }
        }
    }
}
