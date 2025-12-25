using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Kino.PostProcessing;
public class PostProcessing : MonoBehaviour
{
    private Timer timer;
    private PlayerControl playerControl;

    [Header("Vignette")]
    [SerializeField] private float vignetteIntensity;
    [SerializeField] private float vignetteTime;
    private Volume volume;
    private Vignette vignette;

    [Header("Colour Adjustment")]
    private ColorAdjustments colourAdjustment;
    [SerializeField] private float postExposure;
    [SerializeField] private float saturation;
    [SerializeField] private float colourAdjustmentTime;

    [Header("Glitch")]
    private Glitch glitch;
    [SerializeField] private float driftIntensity;
    [SerializeField] private float jitterIntensity;

    private PlayerWallRun wr;
    private PlayerClimb pc;
    private PlayerMovement pm;
    private Rigidbody rb;
    private TimeControlPlayer tcp;
    private PlayerCombatState pcs;

    [Header("Bloom")]
    private Bloom bloom;
    [SerializeField] private float bloomIntensity;
    [SerializeField] private float bloomDecreaseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colourAdjustment);
        volume.profile.TryGet(out glitch);
        volume.profile.TryGet(out bloom);
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManaging>().fromLastScene)
        {
            bloom.intensity.value = bloomIntensity;
            bloom.scatter.value = 0;
        }
        else
        {
            bloom.intensity.value = 1;
            bloom.scatter.value = 1;
        }
        timer = GameObject.Find("Time").GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;

        wr = GameObject.Find("Player").GetComponent<PlayerWallRun>();
        pc = GameObject.Find("Player").GetComponent<PlayerClimb>();
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        tcp = GameObject.Find("Player").GetComponent<TimeControlPlayer>();
        pcs = GameObject.Find("Player").GetComponent<PlayerCombatState>();

        if (wr.isWallRunning || pc.climbing || (pm.crouched && rb.velocity.magnitude > 3f) || (playerControl.Player.Reload.IsPressed() && timer.inPast))
        {
            //vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, vignetteIntensity, vignetteTime * Time.deltaTime);
        }
        else
        {
            //vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0f, vignetteTime * Time.deltaTime);
        }

        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0f, vignetteTime * Time.fixedDeltaTime);

        // get tcp from different player
        tcp = GameObject.Find("Player").GetComponent<TimeControlPlayer>();
        if (tcp.inPast)
        {
            colourAdjustment.postExposure.value = Mathf.Lerp(colourAdjustment.postExposure.value, postExposure, colourAdjustmentTime * Time.deltaTime);
            colourAdjustment.saturation.value = Mathf.Lerp(colourAdjustment.saturation.value, saturation, colourAdjustmentTime * Time.deltaTime);
        }
        else
        {
            colourAdjustment.postExposure.value = 0;
            colourAdjustment.saturation.value = 100;
        }

        glitch.drift.value = Mathf.Lerp(driftIntensity, 0, pcs.health / pcs.maxHealth);
        glitch.jitter.value = Mathf.Lerp(jitterIntensity, 0, pcs.health / pcs.maxHealth);

        bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, bloomIntensity, bloomDecreaseSpeed * Time.deltaTime);
        bloom.scatter.value = Mathf.Lerp(bloom.scatter.value, 0, bloomDecreaseSpeed * Time.deltaTime);
    }

    // message from getting player combat
    public void PlayerDamaged()
    {
        vignette.intensity.value = vignetteIntensity;
    }
}
