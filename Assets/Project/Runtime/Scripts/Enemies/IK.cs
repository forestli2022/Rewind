using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    [SerializeField] private int chainLength = 2;
    [SerializeField] private Transform targetPoint;
    [SerializeField] private Transform hintPoint;

    private float[] boneLengths;
    private float completeLength;
    private Transform[] bones;
    private Vector3[] positions;
    private Vector3[] startDirection;
    private Quaternion[] startRotationBone;
    private Quaternion startRotationTarget;
    private Quaternion startRotationRoot;


    // parameters (don't need to adjust)
    [SerializeField] int iterations = 10;
    [SerializeField] float minDist;
    [Range(0, 1)]
    public float SnapBackStrength = 1f;

    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        bones = new Transform[chainLength + 1];  // . = . = . = .    3 chains, 4 bones
        positions = new Vector3[chainLength + 1];  // position for every bones
        boneLengths = new float[chainLength];  // length of every chain
        completeLength = 0;  // sum of all the bone Lengths
        startDirection = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];
        startRotationTarget = targetPoint.rotation;

        // storing data
        Transform currentTransform = transform;
        for (int i = chainLength; i >= 0; i--)
        {
            bones[i] = currentTransform;
            startRotationBone[i] = currentTransform.rotation;

            if (i == chainLength)
            {
                // special case
                startDirection[i] = targetPoint.position - currentTransform.position;
            }
            else
            {
                startDirection[i] = bones[i + 1].position - currentTransform.position;
                boneLengths[i] = (bones[i + 1].position - currentTransform.position).magnitude;
                completeLength += boneLengths[i];
            }

            currentTransform = currentTransform.parent;  // access parent of this transform
        }
    }

    void LateUpdate()
    {
        // use late update because this script need to wait for all other scripts to finish executing
        Solve();
    }

    private void Solve()
    {
        if (targetPoint == null)
        {
            return;
        }

        if (boneLengths.Length != chainLength)
        {
            Initialize();  // initialize again so that bones can be changed during run time
        }

        // get bone positions
        for (int i = 0; i < bones.Length; i++)
        {
            positions[i] = bones[i].position;  // the reason for having the position as a separate array is that we perform calculation on the position and then set the final result to bones
        }
        Quaternion rootRotation = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        Quaternion rootRotationDiff = rootRotation * Quaternion.Inverse(startRotationRoot);

        // faster using sqrMagnitude since no square root is needed
        if ((targetPoint.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            // if the target position cannot be reached, the chain will be stretched
            Vector3 dir = (targetPoint.position - positions[0]).normalized;
            for (int i = 1; i < positions.Length; i++)
            {
                positions[i] = positions[i - 1] + dir * boneLengths[i - 1];
            }
        }
        else
        {
            // if the bone can reach
            for (int i = 0; i < positions.Length - 1; i++)
            {
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + startDirection[i], SnapBackStrength);
            }

            for (int iter = 0; iter < iterations; iter++)  // repeat solving process for accurate ik
            {
                // back to front
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    if (i == positions.Length - 1)
                    {
                        positions[i] = targetPoint.position;  // last point not moving
                    }
                    else
                    {
                        Vector3 dir = (positions[i] - positions[i + 1]).normalized;
                        positions[i] = positions[i + 1] + dir * boneLengths[i];
                    }
                }

                // fron to back
                for (int i = 1; i < positions.Length; i++)
                {
                    Vector3 dir = (positions[i] - positions[i - 1]).normalized;
                    positions[i] = positions[i - 1] + dir * boneLengths[i - 1];
                }

                // if close enough, break
                if ((positions[positions.Length - 1] - targetPoint.position).sqrMagnitude < minDist * minDist)
                {
                    break;
                }
            }
        }

        // move towards the hint
        if (hintPoint != null)
        {
            for (int i = 1; i < positions.Length - 1; i++)  // move position i
            {
                Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);   // normal, point
                Vector3 projectedHint = plane.ClosestPointOnPlane(hintPoint.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedHint - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        // rotation
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
            {
                // set rotation of the last bone to target rotation
                bones[i].rotation = targetPoint.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            }
            else
            {
                bones[i].rotation = Quaternion.FromToRotation(startDirection[i], positions[i + 1] - positions[i]) * startRotationBone[i];
            }
            bones[i].position = positions[i];
        }

        // set bone positions
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].position = positions[i];
        }
    }
}
