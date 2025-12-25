using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public PlayerControl playerControl;
    [SerializeField] private List<InputActionReference> inputActions;
    [HideInInspector] public InputAction[] buttons;

    void Awake()
    {
        // new controller when scene is loaded
        playerControl = new PlayerControl();
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManaging>().justLoaded)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManaging>().justLoaded = false;
            // load input when first entering the scene
            foreach (InputActionReference inputAction in inputActions)
            {
                string rebind = PlayerPrefs.GetString(inputAction.name, string.Empty);
                if (!string.IsNullOrEmpty(rebind))
                {
                    playerControl.FindAction(inputAction.name).LoadBindingOverridesFromJson(rebind);
                }
            }
        }

        // input checking
        buttons = new InputAction[]{playerControl.Player.MoveLeft, playerControl.Player.MoveRight, playerControl.Player.MoveForward,
        playerControl.Player.MoveBackward, playerControl.Player.Sprint, playerControl.Player.Jump, playerControl.Player.Slide,
        playerControl.Player.Shoot, playerControl.Player.Reload, playerControl.Player.MultiFunction, playerControl.Player.ThrowPistol, playerControl.Player.Rewind};
    }
}
