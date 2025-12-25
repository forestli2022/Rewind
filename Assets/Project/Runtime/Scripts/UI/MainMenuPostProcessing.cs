using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Kino.PostProcessing;
using UnityEngine.Rendering;

public class MainMenuPostProcessing : MonoBehaviour
{
    private ColorAdjustments colourAdjustment;
    private Volume volume;
    private float exposureValue;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out colourAdjustment);
        colourAdjustment.postExposure.value = -100;
        exposureValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // gradually load the game from black to bright
        colourAdjustment.postExposure.value = Mathf.Lerp(colourAdjustment.postExposure.value, exposureValue, 1f * Time.deltaTime);
    }

    // message from continue button
    public void Continue()
    {
        exposureValue = -10;
    }
}
