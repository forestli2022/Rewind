using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolTrigger : MonoBehaviour
{  
    [HideInInspector]
    public bool pistolTriggered;

    void OnTriggerStay(Collider col){
        if(col.gameObject.layer != 6 && col.gameObject.layer != 2){
            pistolTriggered = true;
        }
    }

    void OnTriggerExit(Collider col){
        if(col.gameObject.layer != 6 && col.gameObject.layer != 2){
            pistolTriggered = false;
        }
    }
}
