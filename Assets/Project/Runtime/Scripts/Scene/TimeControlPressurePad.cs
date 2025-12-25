using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlPressurePad : TimeControlParent
{
    [Header("References")]
    [SerializeField] private GameObject greenSign;
    [SerializeField] private GameObject redSign;
    [SerializeField] private Material activatedMat;
    [SerializeField] private Material unactivatedMat;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private AudioSource pressurePadAudio;
    [Header("Collision Detection")]
    [SerializeField] private Vector3 offSet;
    [SerializeField] private Vector3 colliderSize;
    private bool pressured;
    List<bool> pitList = new List<bool>();
    [Header("Activation")]
    [SerializeField] private List<GameObject> relatedItems;


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
        if (pressured && meshRenderer.material != activatedMat)
        {
            meshRenderer.materials = new Material[] { activatedMat };
            greenSign.SetActive(true);
            redSign.SetActive(false);
        }
        else if (!pressured && meshRenderer.material != unactivatedMat)
        {
            greenSign.SetActive(false);
            redSign.SetActive(true);
            meshRenderer.materials = new Material[] { unactivatedMat };
        }
    }

    protected override void Rewinding()
    {
        pressured = pitList[pitList.Count - 1];
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

        // check collision that is currently overlaped by the box. If there are any, pressured = true
        Collider[] col = Physics.OverlapBox(transform.position + offSet, colliderSize / 2, Quaternion.identity);
        if (col.Length != 0)
        {
            bool flag = false;
            foreach (Collider collider in col)
            {
                if (collider.gameObject.layer != 2)
                {  //  if game object is not set to ignore raycast
                    flag = true;
                }
            }
            // if objects detected
            if (!pressured && flag)
            {
                // if pressurd is still false
                pressured = true;
                foreach (GameObject obj in relatedItems)
                {
                    obj.SendMessage("Activate");
                }

                // play audio
                if (!pressurePadAudio.isPlaying)
                {
                    pressurePadAudio.Play();
                }
            }
        }
        else
        {
            if (pressured)
            {
                // if pressured is stil true
                pressured = false;
                foreach (GameObject obj in relatedItems)
                {
                    obj.SendMessage("Deactivate");
                }
            }
        }
        pitList.Add(pressured);
    }

    void OnDrawGizmosSelected()
    {
        // display the box collider
        Utils.DrawBox(transform.position + offSet, Quaternion.identity, colliderSize, Color.black);
    }
}
