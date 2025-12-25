using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindKey : MonoBehaviour
{
    [SerializeField] private InputActionReference inputAction;
    private TextMeshProUGUI text;
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void Start()
    {
        string rebind = PlayerPrefs.GetString(inputAction.name);
        if (!string.IsNullOrEmpty(rebind))
        {
            //asset.LoadBindingOverridesFromJson(rebind);
            inputAction.action.LoadBindingOverridesFromJson(rebind);
        }

        text = transform.Find("Button/Text (TMP)").GetComponent<TextMeshProUGUI>();

        text.text = InputControlPath.ToHumanReadableString(inputAction.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice).ToUpper();
    }

    // message from button
    public void Rebind()
    {
        text.text = "LISTENING...";

        rebindOperation = inputAction.action.PerformInteractiveRebinding()  // caching the rebindOperation here to dispose it later and save memory
        .WithControlsExcluding("Mouse")
        .OnMatchWaitForAnother(0.1f)
        .OnComplete(operation => CompleteRebind())
        .Start();
    }

    private void CompleteRebind()
    {
        rebindOperation.Dispose();
        text.text = InputControlPath.ToHumanReadableString(inputAction.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice).ToUpper();
        PlayerPrefs.SetString(inputAction.name, inputAction.action.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
        // key bindings are saved. In game, it will be loaded by the PlayerInput script
    }

    void Update()
    {
        if (inputAction.action.IsPressed())
        {
            print("pressed");
        }
    }
}
