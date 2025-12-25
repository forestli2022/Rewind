using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform holderTransform;

    [Header("Rotating")]
    [SerializeField] private Transform pistolTransform;
    [SerializeField] private float rotatingSpeed;
    private Rigidbody rb;
    private Timer timer;


    void Start()
    {
        rb = transform.Find("Holder").GetComponent<Rigidbody>();
        timer = GameObject.Find("Time").GetComponent<Timer>();
    }

    void FixedUpdate()
    {
        // rotate pistol model
        pistolTransform.RotateAround(holderTransform.position, transform.up, rotatingSpeed * Time.fixedDeltaTime);
        Ray ray = new Ray(holderTransform.position, Vector3.down);
    }
}
