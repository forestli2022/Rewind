using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeControlTimeline : TimeControlParent
{
    private PlayableDirector timeline;
    private List<double> pitList = new List<double>();  // time line state, 0 means unactivated and paused, >0 means playing, -1 means already activated
    private double currentState;
    private bool activated = false;
    private double currentTime;
    [SerializeField] private List<AudioSource> relatedAudios;

    protected override void StartInit()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    protected override void Rewinding()
    {
        if (relatedAudios.Count > 0 && !relatedAudios[0].mute)
        {
            foreach (AudioSource audios in relatedAudios)
            {
                audios.mute = true;
            }
        }

        currentState = pitList[pitList.Count - 1];
        if (currentState > 0)
        {
            activated = true;
            currentTime = currentState;
            timeline.time = currentTime;
            timeline.Play();
        }
        else
        {
            if (currentState == -1)
            {
                activated = true;
            }
            else
            {
                activated = false;
            }
            timeline.Stop();
        }
        pitList.RemoveAt(pitList.Count - 1);
        firstFrameAfter = true;
    }

    protected override void NotRewinding()
    {
        if (pitList.Count > maxRecordingTime / Time.fixedDeltaTime)
        {
            pitList.RemoveAt(0);
        }
        if (firstFrameAfter)
        {
            foreach (AudioSource audios in relatedAudios)
            {
                audios.mute = false;
            }
            firstFrameAfter = false;
            pitList.Clear();
        }

        // timeline logic
        if (timeline.state == PlayState.Paused)
        {
            if (activated)
            {
                pitList.Add(-1);
            }
            else
            {
                pitList.Add(0);
            }
        }
        else
        {
            pitList.Add(timeline.time);
        }
    }

    // message from activation
    public void Activate()
    {
        if (!activated)
        {
            activated = true;
            timeline.Play();
        }
    }

    public void Deactivate()
    {
        timeline.Stop();
    }
}
