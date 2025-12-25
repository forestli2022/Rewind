using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class PistolBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAnimation pa;
    [SerializeField] private VisualEffect muzzleFlash;
    private PlayerControl playerControl;
    [SerializeField] private PlayerAudio playerAudio;

    [Header("Aim")]
    [SerializeField] private Transform camTrans;
    [SerializeField] private PistolTrigger pt;
    [SerializeField] private Material blueEmissive;
    [SerializeField] private Material redEmissive;

    [Header("State")]
    [SerializeField] private int maxAmmo;
    [SerializeField] private float shootingSpeed;
    public bool ownPistol;
    [HideInInspector] public bool isShooting;
    // these needs to be passed to PIT later
    [HideInInspector] public int bulletNum;
    [HideInInspector] public float timeInterval;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform aim;
    [SerializeField] private GameObject pistol;
    private Camera cam;
    [SerializeField] private float maxShootingDist;

    [Header("Recoil")]
    [SerializeField] private Transform weaponRecoil;
    private Vector3 targetRot;
    private Vector3 currentRot;
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float aimRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimSnappiness;
    [SerializeField] private float aimReturnSpeed;

    [Header("Reload")]
    [SerializeField] private ParticleSystem reloadParticle;
    [SerializeField] private AudioSource reloadingSound;
    [HideInInspector] public bool reloading;
    [HideInInspector] public float currentReloadTime;

    [Header("Throw")]
    public float reloadTime;
    [SerializeField] private Transform trajectoryStartingPoint;
    [SerializeField] private LineRenderer trajectory;
    [SerializeField] private float trajectoryTimeStep;
    private GameObject pistolPickup;
    [SerializeField] private float throwingForce;
    [SerializeField] private LayerMask excludePlayer;
    private bool throwLastPressed;
    private List<Vector3> trajectoryPoints = new List<Vector3>();

    [Header("Picking Up")]
    [SerializeField] private float timeToPickup;
    [SerializeField] private float pickupDistance;
    private float cntTime;


    // clone modification
    private bool isClone;
    private PITInteractions pit;
    private ClonedPlayer cp;

    // Start is called before the first frame update
    void Start()
    {
        bulletNum = maxAmmo;
        pistolPickup = GameObject.Find("Environment/Player Related/Pistol Pickup/Holder");

        playerControl = GetComponent<PlayerInput>().playerControl;

        isClone = gameObject.tag == "Clone";
        if (isClone)
        {
            cp = GetComponent<ClonedPlayer>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Aim();
        // item pickup
        cntTime += Time.fixedDeltaTime; // increment time after throwing a pistol;
        Pickup();

        if (!ownPistol)
        {
            if (pistol.activeInHierarchy)
            {
                pistol.SetActive(false);
            }
            return;
        }


        isClone = gameObject.tag == "Clone";
        // pit for clone
        if (isClone)
        {
            pit = cp.pit;
        }

        // aiming
        // resetting the rotation caused by recoil
        if (pa.aiming)
        {
            targetRot = Vector3.Lerp(targetRot, Vector3.zero, aimReturnSpeed * Time.fixedDeltaTime);
            currentRot = Vector3.Slerp(currentRot, targetRot, aimSnappiness * Time.fixedDeltaTime);
        }
        else
        {
            targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.fixedDeltaTime);
            currentRot = Vector3.Slerp(currentRot, targetRot, snappiness * Time.fixedDeltaTime);
        }

        weaponRecoil.localRotation = Quaternion.Euler(currentRot);

        // reloading / shooting
        if (Utils.keyPressed(playerControl.Player.Shoot, pit, isClone) && pistol.activeInHierarchy && pa.canShoot)
        {
            if (timeInterval >= shootingSpeed && bulletNum > 0 && !reloading)
            {
                Shoot();
                Recoil();
                bulletNum -= 1;
                timeInterval = 0;
                isShooting = true;
            }
        }
        else
        {
            isShooting = false;
        }

        Reload();

        // throwing 
        if (Utils.keyPressed(playerControl.Player.ThrowPistol, pit, isClone) && pistol.activeInHierarchy && pa.canShoot)
        {
            throwLastPressed = true;
            AimThrow();
        }

        if (!Utils.keyPressed(playerControl.Player.ThrowPistol, pit, isClone) && throwLastPressed && pa.canShoot)
        {
            trajectory.positionCount = 0;
            cntTime = 0;
            throwLastPressed = false;
            Throw();
        }

        timeInterval += Time.fixedDeltaTime;
    }

    private void Aim()
    {
        Ray ray = new Ray(camTrans.position, camTrans.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxShootingDist) && (!ownPistol || !pt.pistolTriggered))
        {

            aim.position = hit.point;
        }
        else
        {
            aim.position = camTrans.position + camTrans.forward * maxShootingDist;
        }
    }

    private void Shoot()
    {
        GameObject b = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
        b.SetActive(true);
        b.transform.forward = aim.position - bulletSpawnPoint.position;

        // vfx
        muzzleFlash.Play();

        // sound
        pistol.GetComponent<AudioSource>().Play();
    }

    private void Recoil()
    {
        Random.InitState(bulletNum);
        if (pa.aiming)
        {
            targetRot += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), 0);
        }
        else
        {
            targetRot += new Vector3(recoilX, Random.Range(-recoilY, recoilY), 0);
        }

    }

    private void Reload()
    {
        // setting appearence of the pistol
        if (bulletNum <= 3)
        {
            pistol.GetComponent<Renderer>().material = redEmissive;
        }
        else
        {
            pistol.GetComponent<Renderer>().material = blueEmissive;
        }


        // setting reloading state
        if ((bulletNum <= 0 || (Utils.keyPressed(playerControl.Player.Reload, pit, isClone)) && !reloading) && pistol.activeInHierarchy)
        {
            reloading = true;
        }
        // if player is reloading, perform these actions
        if (reloading)
        {
            currentReloadTime += Time.fixedDeltaTime;
            if (currentReloadTime >= reloadTime - 0.7f)
            {
                bulletNum = maxAmmo;
                reloadParticle.Stop();
                reloadingSound.Stop();
            }
            else
            {
                if (!reloadParticle.isPlaying)
                {
                    reloadParticle.Play();
                    reloadingSound.Play();
                }
            }
            if (currentReloadTime >= reloadTime)
            {
                reloading = false;
                currentReloadTime = 0;
            }
        }

    }

    private void AimThrow()
    {
        Rigidbody pistolRb = pistolPickup.GetComponent<Rigidbody>();

        Vector3 localForceVector = throwingForce * (aim.position - trajectoryStartingPoint.position).normalized;
        Vector3 velocity = (localForceVector / pistolRb.mass) * Time.fixedDeltaTime;
        trajectoryPoints.Clear();


        int i = 0;
        Vector3 currentPoint;
        do
        {
            float timePassed = trajectoryTimeStep * i;
            Vector3 movementVector = new Vector3(
                velocity.x * timePassed,
                velocity.y * timePassed + 0.5f * Physics.gravity.y * timePassed * timePassed,  // u * t + 1/2 * a * t^2
                velocity.z * timePassed
            );
            currentPoint = trajectoryStartingPoint.position + movementVector;
            trajectoryPoints.Add(currentPoint);
            i++;
        } while (!Physics.CheckSphere(currentPoint, pistolPickup.transform.localScale.x, excludePlayer) && i < (int)(10 / trajectoryTimeStep));

        trajectory.positionCount = trajectoryPoints.Count;
        trajectory.SetPositions(trajectoryPoints.ToArray());
    }

    private void Throw()
    {
        Vector3 localForceVector = throwingForce * (aim.position - trajectoryStartingPoint.position).normalized;
        ownPistol = false;
        pistol.SetActive(false);
        pistolPickup.SetActive(true);
        pistolPickup.transform.position = trajectoryStartingPoint.position;
        pistolPickup.GetComponent<Rigidbody>().AddForce(localForceVector, ForceMode.Force);

        // fix the bug of animator rigging component
        transform.Find("Armature/Armature@T-Pose").gameObject.GetComponent<RigBuilder>().enabled = false;
        transform.Find("Armature/Armature@T-Pose").gameObject.GetComponent<RigBuilder>().enabled = true;
    }

    private void Pickup()
    {
        if (!GameObject.Find("Time").GetComponent<Timer>().rewinding && Vector3.Distance(transform.position, pistolPickup.transform.position) <= pickupDistance && cntTime >= timeToPickup && pistolPickup.activeInHierarchy)
        {
            ownPistol = true;
            pistol.SetActive(true);
            pistolPickup.SetActive(false);
            // fix the bug of animator rigging component
            transform.Find("Armature/Armature@T-Pose").gameObject.GetComponent<RigBuilder>().enabled = false;
            transform.Find("Armature/Armature@T-Pose").gameObject.GetComponent<RigBuilder>().enabled = true;

            playerAudio.PlayPickupAudio();
        }
    }
}
