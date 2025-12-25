using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour
{
    // game object references
    [SerializeField] private Transform head;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject lockedCam;
    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject bot;

    // rotation
    [HideInInspector] public float mouseX;
    [HideInInspector] public float mouseY;
    float multiplier = 0.01f;
    [HideInInspector] public float xRotation;
    [HideInInspector] public float yRotation;
    [HideInInspector] public float xLim;
    [HideInInspector] public bool freeToRotate;

    // script references
    private PlayerWallRun wr;
    private PlayerMovement pm;
    private TimeControlPlayer tcp;
    [Header("Custom pass")]
    [HideInInspector] public CustomPassVolume customPassVolume;

    [Header("Sensitivity setting")]
    [SerializeField] private float sensitivity;
    [SerializeField] private Slider slider;

    // clone modification
    [HideInInspector] public bool isClone = false;

    private void Start()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        freeToRotate = true;
        pm = GetComponent<PlayerMovement>();
        wr = GetComponent<PlayerWallRun>();
        tcp = GetComponent<TimeControlPlayer>();

        customPassVolume = GameObject.Find("Environment/Lightings/Custom Pass").GetComponent<CustomPassVolume>();

        // load sensitivity
        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        if (sensitivity == 0)
        {
            sensitivity = 100;  // set to default value;
        }
        slider.value = sensitivity;


        if (!isClone)
        {
            // rotate player for seamless transition
            GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
            xRotation = gameManager.GetComponent<GameManaging>().rotX;
            yRotation = gameManager.GetComponent<GameManaging>().rotY;
        }
        else
        {
            isClone = false;
        }
    }

    private void Update()
    {
        if (freeToRotate)
        {
            MouseInput();
            if (!cam.activeInHierarchy || lockedCam.activeInHierarchy)
            {
                lockedCam.SetActive(false);
            }
        }
        else
        {
            if (cam.activeInHierarchy || !lockedCam.activeInHierarchy)
            {
                lockedCam.SetActive(true);
                lockedCam.GetComponent<Camera>().fieldOfView = cam.transform.Find("Recoil/Main Camera").GetComponent<Camera>().fieldOfView;
            }
        }
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        if (!pm.crouched && !wr.isWallRunning && freeToRotate)
        {
            bot.transform.rotation = orientation.rotation;
        }

        if (freeToRotate)
        {
            cam.transform.rotation = Quaternion.Euler(Mathf.Clamp(xRotation, -xLim, xLim), yRotation, wr.tilt);
        }

        // custom pass
        if (tcp.inPast)
        {
            customPassVolume.customPasses[0].enabled = true;
            customPassVolume.targetCamera = Camera.main;
        }
        else
        {
            customPassVolume.customPasses[0].enabled = false;
        }
    }

    private void MouseInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * Time.timeScale;
        mouseY = Input.GetAxisRaw("Mouse Y") * Time.timeScale;
        yRotation += mouseX * sensitivity * multiplier;
        xRotation -= mouseY * sensitivity * multiplier;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
    }

    // message from sensitivity slider
    public void AdjustSensitivity(Slider slider)
    {
        sensitivity = slider.value;
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.Save();
    }
}
