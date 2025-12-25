using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCube : MonoBehaviour
{
    private PlayerControl playerControl;
    [SerializeField] private Transform aim;
    [SerializeField] private Transform cam;
    [SerializeField] private float pickUpDistance;
    [SerializeField] private float minPlacingDistance;
    [HideInInspector] public GameObject hologramCube;
    [HideInInspector] public bool hasCube;
    [HideInInspector] public bool canPickUp;
    private Transform cubeCollection;  // instead of disabling the cube, simply teleport them into an unseen place so the time control script works normally
    private bool keyDown;
    [HideInInspector] public GameObject currentCube = null;

    // clone modification
    private bool isClone;
    private PITInteractions pit;
    private ClonedPlayer cp;

    // Start is called before the first frame update
    void Start()
    {
        playerControl = GetComponent<PlayerInput>().playerControl;
        isClone = gameObject.tag == "Clone";
        if (isClone)
        {
            cp = GetComponent<ClonedPlayer>();
        }
        cubeCollection = GameObject.Find("Environment/Static Objects/CubeCollectionPoint").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isClone = gameObject.tag == "Clone";
        // pit for clone
        if (isClone)
        {
            pit = cp.pit;
        }


        if (!hasCube)
        {
            if (hologramCube.activeInHierarchy)
            {
                hologramCube.SetActive(false);
            }

            if (Physics.Raycast(cam.position, aim.position - cam.position, out RaycastHit hit, pickUpDistance) && hit.transform.gameObject.tag == "Cube")
            {
                canPickUp = true;
                // grab cube
                if (Utils.keyPressed(playerControl.Player.MultiFunction, pit, isClone && Vector3.Distance(aim.position, cam.position) < pickUpDistance) && !keyDown)
                {
                    currentCube = hit.transform.gameObject;
                    currentCube.transform.position = cubeCollection.position;
                    currentCube.GetComponent<Rigidbody>().isKinematic = true;
                    currentCube.GetComponent<Rigidbody>().drag = 0;
                    currentCube.GetComponent<Rigidbody>().drag = 0.05f;
                    hasCube = true;
                    canPickUp = false;
                }
            }
            else
            {
                canPickUp = false;
            }
        }
        else
        {
            canPickUp = false;
            if (!hologramCube.activeInHierarchy)
            {
                hologramCube.SetActive(true);
            }

            // the cube cannot be placed too near
            if (hologramCube.activeInHierarchy && Vector3.Distance(aim.position, cam.position) < minPlacingDistance)
            {
                hologramCube.SetActive(false);
                return;
            }

            // positioning cube
            if (Physics.Raycast(cam.position, aim.position - cam.position, out RaycastHit hit, pickUpDistance + 1))
            {
                // +1 because cube's side length is 2
                hologramCube.transform.forward = hit.normal;
                hologramCube.transform.position = hit.point + hit.normal;
            }
            else
            {
                hologramCube.transform.position = cam.position + (aim.position - cam.position).normalized * pickUpDistance;
                hologramCube.transform.forward = (hologramCube.transform.position - cam.position).normalized;
            }

            // place cube
            if (Utils.keyPressed(playerControl.Player.MultiFunction, pit, isClone) && !keyDown)
            {
                currentCube.GetComponent<Rigidbody>().isKinematic = false;
                currentCube.transform.position = hologramCube.transform.position;
                currentCube.transform.rotation = hologramCube.transform.rotation;
                currentCube = null;
                hasCube = false;
            }
        }

        // simulate key down
        keyDown = Utils.keyPressed(playerControl.Player.MultiFunction, pit, isClone);
    }

    void OnDisable()  // when disabled, disable hologram and set the hasCube to false
    {
        hologramCube.SetActive(false);
        hasCube = false;
    }

    void OnDestroy()
    {
        if (currentCube != null)
        {
            currentCube.transform.position = hologramCube.transform.position;
        }
    }
}
