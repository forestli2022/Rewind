using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeControlTerminal : TimeControlParent
{
    [SerializeField] private TextMeshProUGUI activationSign;
    [SerializeField] private Color unactiveColour;
    [SerializeField] private Color activeColour;
    List<bool> pitList = new List<bool>();
    private bool currentlyActivated;

    // a list for the different things terminal controls
    [SerializeField] private List<GameObject> relatedItems = new List<GameObject>();
    // audio
    private AudioSource activationSound;

    protected override void StartInit()
    {
        activationSound = GetComponent<AudioSource>();
    }

    protected override void FixedUpdate()
    {
        if (timer.rewinding)
        {
            Rewinding();
        }
        else
        {
            NotRewinding();
        }

        // terminal logic
        if (currentlyActivated)
        {
            if (activationSign.text != "Activated")
            {
                // the first frame where text hasn't been set yet
                activationSign.text = "Activated :)";
                activationSign.color = activeColour;
            }
        }
        else
        {
            activationSign.text = "Unactivated";
            activationSign.color = unactiveColour;
        }
    }

    protected override void Rewinding()
    {
        currentlyActivated = pitList[pitList.Count - 1];
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
            firstFrameAfter = false;
            pitList.Clear();
        }
        pitList.Add(currentlyActivated);
    }

    public void ActivateTerminal(bool activated)
    {
        currentlyActivated = activated;
        if (activated)
        {
            activationSound.Play();
        }

        // activate all related objects
        foreach (GameObject obj in relatedItems)
        {
            obj.SendMessage("Activate");
        }
    }

    public bool IsActivated()
    {
        return currentlyActivated;
    }
}
