using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlDoor : TimeControlParent
{
    [Header("Activation")]
    [SerializeField] private bool activated;
    [HideInInspector] public bool opening;

    [Header("References")]
    [SerializeField] private Material lightMat;
    [SerializeField] private Material activatedMat;
    [SerializeField] private Material unactivatedMat;
    private MeshRenderer meshRenderer;
    private List<bool> pitList = new List<bool>();
    // audio
    private AudioSource gateOpeningAudio;

    // collision detection
    [Header("Collision Detection")]
    [SerializeField] private Vector3 offSet;
    [SerializeField] private Vector3 colliderSize;
    [SerializeField] private LayerMask playerOnly;

    protected override void StartInit()
    {
        // references
        meshRenderer = GetComponent<MeshRenderer>();
        gateOpeningAudio = transform.parent.GetComponent<AudioSource>();

        if (activated)
        {
            meshRenderer.materials = new Material[] { lightMat, activatedMat };
        }
        else
        {
            meshRenderer.materials = new Material[] { lightMat, unactivatedMat };
        }
    }

    protected override void FixedUpdate()  // override fixed update so that additional material setting can be added.
    {
        if (timer.rewinding)
        {
            Rewinding();
        }
        else
        {
            NotRewinding();
        }

        // change material
        if (activated && meshRenderer.material != activatedMat)
        {
            meshRenderer.materials = new Material[] { lightMat, activatedMat };
        }
        else if (!activated && meshRenderer.material != unactivatedMat)
        {
            meshRenderer.materials = new Material[] { lightMat, unactivatedMat };
        }
    }

    protected override void Rewinding()
    {
        activated = pitList[pitList.Count - 1];
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
        pitList.Add(activated);

        // check collision with the player
        Collider[] col = Physics.OverlapBox(transform.position + offSet, colliderSize / 2, Quaternion.identity, playerOnly);
        if (col.Length != 0 && activated)
        {
            // if players detected
            if (!opening)
            {
                // if opening is still false
                opening = true;
                if (!gateOpeningAudio.isPlaying)
                {
                    gateOpeningAudio.Play();
                }
            }
        }
        else
        {
            if (opening)
            {
                // if opening is stil true
                opening = false;
            }
        }
        pitList.Add(activated);
    }

    // message from terminal
    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }

    void OnDrawGizmosSelected()
    {
        // display the box collider
        Utils.DrawBox(transform.position + offSet, Quaternion.identity, colliderSize, Color.black);
    }
}
