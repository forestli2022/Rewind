using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TimeControlMech : TimeControlParent
{
    [Header("References")]
    [SerializeField] private MechShoot ms;
    [SerializeField] private MechNavigation mn;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private MechView mv;
    [SerializeField] private MechIKFootSolver leftmf;
    [SerializeField] private MechIKFootSolver rightmf;
    [SerializeField] private MechRoutine mr;
    [SerializeField] private MechCombatState mcs;
    [SerializeField] private GameObject holder;
    private AudioSource[] mechSounds;

    // rewinding
    private List<PITMech> pitList = new List<PITMech>();

    // state variables
    private bool agentEnabled;
    private bool alert;
    private float progress, lastProgress, routineProgress, timeNotSeen;
    private float health;
    private bool dead;
    private float secondsAfterFinished;

    protected override void StartInit()
    {
        mechSounds = GetComponents<AudioSource>();
    }


    protected override void Rewinding()
    {
        ms.enabled = false;
        mn.enabled = false;
        mv.enabled = false;
        agent.enabled = false;
        leftmf.enabled = false;
        rightmf.enabled = false;
        mcs.explosionSound.Stop();
        mcs.countDownSound.Stop();
        mcs.enabled = false;
        mechSounds[1].Stop();

        // store foot solver state in variables, so it could be assigned in the first frame after finish rewinding
        pitList[pitList.Count - 1].SetState(out agentEnabled, out alert, out progress, out lastProgress, out routineProgress, out timeNotSeen, out health, out dead, out secondsAfterFinished);
        pitList.RemoveAt(pitList.Count - 1);
        firstFrameAfter = true;  // set first frame after to true so when R released, clear PIT list

        // asign some of the values here for alert sign behaviour
        ms.alert = alert;
        mv.timeNotSeen = timeNotSeen;

        if (alert)
        {
            mv.light.GetComponent<Light>().color = mv.battleColor;
        }
        else
        {
            mv.light.GetComponent<Light>().color = mv.normalColor;
        }

        // activate holder of mech if not dead
        if (secondsAfterFinished < mcs.explosionTime)
        {
            holder.SetActive(true);
        }
    }

    protected override void NotRewinding()
    {
        if (pitList.Count > maxRecordingTime / Time.fixedDeltaTime)
        {
            pitList.RemoveAt(0);
        }
        if (firstFrameAfter)
        {
            mcs.enabled = true;
            mcs.dead = dead;
            mcs.secondsAfterFinished = secondsAfterFinished;
            if (secondsAfterFinished >= 0 && secondsAfterFinished < mcs.explosionTime)
            {
                mcs.countDownSound.time = secondsAfterFinished;
                mcs.countDownSound.Play();
            }
            else if (secondsAfterFinished >= mcs.explosionTime && holder.activeInHierarchy)
            {
                mcs.explosionSound.time = secondsAfterFinished - mcs.explosionTime;
                mcs.explosionSound.Play();
            }
            // assign state
            if (!dead)
            {
                ms.enabled = true;
                mn.enabled = true;
                mv.enabled = true;
                agent.enabled = agentEnabled;
                leftmf.enabled = true;
                rightmf.enabled = true;

                ms.alert = alert;
                ms.progress = progress;
                ms.lastProgress = lastProgress;
                mv.timeNotSeen = timeNotSeen;
                mcs.health = health;

                if (!alert)
                {
                    mr.enabled = true;
                    mr.routineProgress = routineProgress;
                }
            }

            firstFrameAfter = false;
            pitList.Clear();
        }
        pitList.Add(new PITMech(agent.enabled, ms.alert, ms.progress, ms.lastProgress, mr.routineProgress, mv.timeNotSeen, mcs.health, mcs.dead, mcs.secondsAfterFinished));
    }
}
