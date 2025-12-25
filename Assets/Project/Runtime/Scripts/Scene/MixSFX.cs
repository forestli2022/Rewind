using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixSFX : MonoBehaviour
{
    // adjust the sound effects during rewinding
    public AudioMixer masterMixer;
    private TimeControlPlayer tcp;
    private PlayerCombatState pcs;

    public int cutOffFreq;
    public int room;        // these two variables produces underwater sound effects

    public float rewindTime;

    private void FixedUpdate()
    {
        GameObject player = GameObject.Find("Player");
        tcp = player.GetComponent<TimeControlPlayer>();
        pcs = player.GetComponent<PlayerCombatState>();

        float cof;
        float r;
        masterMixer.GetFloat("cutoff freq", out cof);
        masterMixer.GetFloat("room", out r);
        if (tcp.inPast || pcs.dead)
        {
            masterMixer.SetFloat("cutoff freq", Mathf.Lerp(cof, cutOffFreq, rewindTime * Time.fixedDeltaTime));
            masterMixer.SetFloat("room", Mathf.Lerp(r, room, rewindTime * Time.fixedDeltaTime));
        }
        else
        {
            masterMixer.SetFloat("cutoff freq", Mathf.Lerp(cof, 22000, rewindTime * Time.fixedDeltaTime));
            masterMixer.SetFloat("room", Mathf.Lerp(r, -10000, rewindTime * Time.fixedDeltaTime));
        }
    }
}
