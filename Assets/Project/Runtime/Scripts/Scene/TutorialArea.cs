using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialArea : MonoBehaviour
{
    [SerializeField] private string tutorialText;

    void Start()  // this is to monitor the change in key bindings in realtime. If the key binding changes, the tutorial still works dynamically
    {
        InputAction[] buttons = GameObject.Find("Player").GetComponent<PlayerInput>().buttons;

        // run the decoding algorithm n times depends on number of curely brackets
        int i = 0;
        while (tutorialText.Contains('{') && i < 10)
        {
            i++;
            string key = tutorialText.Substring(tutorialText.IndexOf('{') + 1, tutorialText.IndexOf('}') - tutorialText.IndexOf('{') - 1);

            foreach (InputAction button in buttons)
            {
                if (button.name.Equals(key))
                {
                    string keyBinding = button.bindings[0].effectivePath;
                    string boundKey = keyBinding.Substring(keyBinding.IndexOf('/') + 1, keyBinding.Length - 1 - keyBinding.IndexOf('/')).ToUpper();
                    tutorialText = tutorialText.Substring(0, tutorialText.IndexOf('{')) + '[' + boundKey + ']' + tutorialText.Substring(tutorialText.IndexOf('}') + 1, tutorialText.Length - 1 - tutorialText.IndexOf('}'));

                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 6)
        {
            GameObject.Find("Player").SendMessage("PlayTutorial", tutorialText);
            gameObject.SetActive(false);
        }
    }
}
