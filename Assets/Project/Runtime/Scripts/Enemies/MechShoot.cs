using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class MechShoot : MonoBehaviour
{
    [HideInInspector] public float progress = 0, lastProgress = 0;
    [SerializeField] private GameObject bullet;
    [HideInInspector] public bool alert = false;  // if alert, mech will chase player/player clone

    [Header("Bullet related")]
    [SerializeField] private float reloadTime = 3;
    [SerializeField] private float shootingTime;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform turret;
    [SerializeField] private float shootInterval;

    [SerializeField] private float strayFactor;
    [SerializeField] private float shootingRange;
    [SerializeField] private GameObject bm;
    // muzzle flash
    [SerializeField] private VisualEffect muzzle;
    MechNavigation mechNav;
    [SerializeField] private MechView mechView;
    [SerializeField] private TimeControlVFX tcv;
    private float vfxCnt = 0;
    [SerializeField] private float muzzleFlashLifeTime = 0.08f;
    private bool muzzleFlashPlaying = false;
    // control routine script
    [SerializeField] private MechRoutine mr;

    // audio
    [HideInInspector] public AudioSource[] mechSound;

    // light
    [SerializeField] private GameObject shootingLight;
    [SerializeField] private float shootingLightLife;

    // alert range
    [SerializeField] private float alertRange;

    // Start is called before the first frame update
    void Start()
    {
        mechNav = GetComponent<MechNavigation>();
        mechSound = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (alert)
        {
            // play audio first
            if (mr.enabled)
            {
                mr.enabled = false;
                mechSound[0].Play();
            }

            // calculating current progress
            progress += Time.fixedDeltaTime;
            if (mechNav.targetPlayer == null)
            {
                mechNav.targetPlayer = GameObject.Find("Player");
            }
            Ray ray = new Ray(bulletSpawnPoint.position, mechNav.targetPlayer.transform.position - bulletSpawnPoint.position);
            if (progress >= reloadTime && Physics.Raycast(ray, out RaycastHit hit, shootingRange) && hit.transform.gameObject.layer == 6)
            { // if the player is within shooting range and can be seen
                // ready to shoot
                if (progress - lastProgress >= shootInterval)
                {
                    GameObject b = Instantiate(bullet, bulletSpawnPoint.position, turret.rotation);
                    b.SetActive(true);
                    b.GetComponent<MechBulletBehaviour>().bulletManager = bm.GetComponent<BulletManager>();

                    // variation on bullet spawn position, reproduceable random
                    Random.InitState((int)progress);
                    float randomX = Random.Range(-strayFactor, strayFactor);
                    Random.InitState((int)(progress * 10));
                    float randomY = Random.Range(-strayFactor, strayFactor);
                    Random.InitState((int)(progress * 100));
                    float randomZ = Random.Range(-strayFactor, strayFactor);
                    b.transform.Rotate(randomX, randomY, randomZ);
                    lastProgress = progress;

                    // sound
                    if (!mechSound[1].isPlaying)
                    {
                        mechSound[1].Play();
                    }

                    // vfx
                    if (vfxCnt < muzzleFlashLifeTime)
                    {
                        tcv.VFXIsPlaying();
                    }
                    muzzle.Play();

                    // light
                    shootingLight.SetActive(true);
                }
                else if (progress - lastProgress >= shootingLightLife)
                {
                    shootingLight.SetActive(false);
                }
                else
                {
                    shootingLight.SetActive(false);
                }
                if (muzzleFlashPlaying)
                {
                    vfxCnt += Time.fixedDeltaTime;  // increment cnt to calculate muzzle lifetime
                    if (vfxCnt >= muzzleFlashLifeTime)
                    {  // vfx should be destroyed by now
                        tcv.pitList[tcv.pitList.Count - 1] = true;  // destroy this frame
                    }
                }

                if (progress >= reloadTime + shootingTime)
                {
                    // end shooting and loop back to reloading stage
                    progress = 0;
                    lastProgress = 0;

                    mechSound[1].Stop();
                }
            }
            else
            {
                // stop shooting
                if (mechSound[1].isPlaying)
                {
                    mechSound[1].Stop();
                }
                shootingLight.SetActive(false);
            }

        }
        else
        {
            // check surroundings, if any mech is alert in a given range, becomes alert and has the same target as that mech
            foreach (Collider collider in Physics.OverlapSphere(transform.position, alertRange))
            {
                if (collider.gameObject.layer == 7 && collider.transform.parent.parent.GetComponent<MechShoot>().alert)
                {  // is an enemy and is alert 

                    MechNavigation neighbourMechNav = collider.transform.parent.parent.GetComponent<MechNavigation>();
                    if (neighbourMechNav.mechView.timeNotSeen < neighbourMechNav.mechView.timeTargetLost - 0.5f)
                    {  // make sure mechs don't alert each other in a loop
                        alert = true;
                        mechNav.targetPlayer = neighbourMechNav.targetPlayer;
                        mechView.timeNotSeen = neighbourMechNav.mechView.timeNotSeen;
                        mechView.GetComponent<Light>().color = mechView.battleColor;
                        return;
                    }
                }
            }

            // if not alert, progress is always 0, mech follows the routine
            if (progress != 0 || !mr.enabled || mechSound[1].isPlaying || shootingLight.activeInHierarchy)
            {
                mr.enabled = true;
                progress = 0;
                mechSound[1].Stop();
                shootingLight.SetActive(false);
            }
        }
    }

}
