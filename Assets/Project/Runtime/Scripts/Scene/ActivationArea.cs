using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationArea : MonoBehaviour
{
    [SerializeField] private List<GameObject> relatedItems;
    [SerializeField] private List<GameObject> pauseItems;
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 6)
        {
            // if is player
            // activate all related objects
            foreach (GameObject obj in relatedItems)
            {
                obj.SendMessage("Activate");
            }
            foreach (GameObject obj in pauseItems)
            {
                obj.SendMessage("Deactivate");
            }
        }
    }
}
