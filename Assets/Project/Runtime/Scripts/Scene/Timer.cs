using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("References")]
    private TimeControlPlayer tcp;
    private PlayerControl playerControl;
    [Header("Time settings")]
    public float fastForwardScale;
    public float maximumRecordingTime;
    public float coolDown;
    [HideInInspector] public float rewindTime;
    [HideInInspector] public bool inPast;
    [HideInInspector] public bool rewinding;
    [HideInInspector] public float cd;
    private float fixedDeltaTime;

    // Start is called before the first frame update
    void Start()
    {
        cd = 0;
        // initialize player combat state
        PlayerCombatState pcs = GameObject.Find("Player").GetComponent<PlayerCombatState>();
        pcs.health = pcs.maxHealth;

        // make a copy of fixed delta time
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;

        tcp = GameObject.Find("Player").GetComponent<TimeControlPlayer>();
        inPast = tcp.inPast;
        if ((inPast && !rewinding) || tcp.pitList.Count == 0)
        {
            cd = coolDown;
        }
        else if (!inPast)
        {
            cd -= Time.fixedDeltaTime;
        }

        if (cd < 0)
        {
            cd = 0;
        }

        // rewind time setting, showing player how long they have rewinded
        if (rewinding)
        {
            rewindTime += Time.fixedDeltaTime;
        }
        else if (inPast)
        {
            rewindTime -= Time.fixedDeltaTime;
        }
        else if (rewindTime != 0)
        {
            rewindTime = 0;
        }
    }
}
