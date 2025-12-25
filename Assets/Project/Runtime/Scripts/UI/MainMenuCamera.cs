using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    private float mouseX;
    private float mouseY;
    private float xRotation;
    private float yRotation;

    [SerializeField] private float multiplier;
    [SerializeField] private float returnSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        yRotation += mouseX * multiplier;
        xRotation -= mouseY * multiplier;

        xRotation = Mathf.Lerp(xRotation, 0, returnSpeed * Time.deltaTime);
        yRotation = Mathf.Lerp(yRotation, 0, returnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
