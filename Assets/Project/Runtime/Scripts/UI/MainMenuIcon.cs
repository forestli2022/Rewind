using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuIcon : MonoBehaviour
{

    [SerializeField] private int notRewinding;
    private bool rotating;
    private float timeCnt;
    [SerializeField] private Image filler1;
    [SerializeField] private Image filler2;
    [SerializeField] private float hourGlassFillSpeed;
    [SerializeField] private float hourGlassRotatingSpeed;
    [SerializeField] private ParticleSystem iconParticles;
    [SerializeField] private ParticleSystem iconRewindingParticles;
    [SerializeField] private Color normalColour;
    [SerializeField] private Color rewindingColour;

    // referencing the mainMenuCanvas
    [SerializeField] private MainMenuCanvas mainMenuCanvas;
    // referencing the transform of whole icon
    [SerializeField] private Transform wholeIconTransform;

    private bool period1 = true;   // indicate which rotating period the hourglass is in

    // Start is called before the first frame update
    void Start()
    {
        iconParticles.startLifetime = 3f;
        iconRewindingParticles.startLifetime = 0f;
        GetComponent<Image>().material.SetColor("_EmissiveColor", normalColour * 21);
        filler1.GetComponent<Image>().material.SetColor("_EmissiveColor", normalColour * 21);
        filler2.GetComponent<Image>().material.SetColor("_EmissiveColor", normalColour * 21);
        mainMenuCanvas = GameObject.Find("Canvas").GetComponent<MainMenuCanvas>();
    }

    // Update is called once per frame
    void Update()
    {
        // detect change in pages
        if (mainMenuCanvas.currentPage != 0)
        {
            notRewinding = -1;
        }
        else
        {
            notRewinding = 1;
        }


        // position icon
        if (mainMenuCanvas.currentPage != 1)
        {
            wholeIconTransform.localPosition = Vector3.Lerp(wholeIconTransform.localPosition, new Vector3(222, -13, 0), 7 * Time.deltaTime);
        }
        else
        {
            wholeIconTransform.localPosition = Vector3.Lerp(wholeIconTransform.localPosition, new Vector3(0, -13, 0), 7 * Time.deltaTime);
        }


        // particle system
        if (notRewinding == 1 && iconParticles.startLifetime != 3)
        {
            iconParticles.startLifetime = 3f;
            iconRewindingParticles.startLifetime = 0f;
            GetComponent<Image>().material.SetColor("_EmissiveColor", normalColour * 21);
            filler1.GetComponent<Image>().material.SetColor("_EmissiveColor", normalColour * 21);
            filler2.GetComponent<Image>().material.SetColor("_EmissiveColor", normalColour * 21);
        }
        else if (notRewinding == -1 && iconParticles.startLifetime == 3)
        {
            iconParticles.startLifetime = 0f;
            iconRewindingParticles.startLifetime = 3f;
            GetComponent<Image>().material.SetColor("_EmissiveColor", rewindingColour * 21);
            filler1.GetComponent<Image>().material.SetColor("_EmissiveColor", rewindingColour * 21);
            filler2.GetComponent<Image>().material.SetColor("_EmissiveColor", rewindingColour * 21);
        }

        if (filler1.transform.position.y > filler2.transform.position.y)
        {
            filler1.fillOrigin = 1;
            filler2.fillOrigin = 0;
            filler1.fillAmount -= hourGlassFillSpeed * Time.deltaTime * notRewinding;
            if ((notRewinding == 1 && filler1.fillAmount <= 0) || (notRewinding == -1 && filler1.fillAmount >= 1))
            {
                rotating = true;
            }
        }
        else
        {
            filler1.fillOrigin = 0;
            filler2.fillOrigin = 1;
            filler1.fillAmount += hourGlassFillSpeed * Time.deltaTime * notRewinding;
            if ((notRewinding == 1 && filler1.fillAmount >= 1) || (notRewinding == -1 && filler1.fillAmount <= 0))
            {
                rotating = true;
            }
        }

        if (rotating)
        {
            timeCnt += Time.deltaTime * notRewinding;
            transform.Rotate(new Vector3(0, 0, -hourGlassRotatingSpeed * Time.deltaTime * notRewinding));
            if (notRewinding * timeCnt >= 180 / (hourGlassRotatingSpeed))
            {
                rotating = false;
                timeCnt = 0;
                if (Mathf.Abs(transform.eulerAngles.z - 180) < 5)
                {
                    transform.eulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
        }

        filler2.fillAmount = 1 - filler1.fillAmount;
    }
}
