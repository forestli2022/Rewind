using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertSignBehaviour : MonoBehaviour
{
    Camera cam;
    public Transform alertSignTransform;
    RectTransform rectTransform;

    public MechShoot ms;
    public MechView mv;
    [HideInInspector]
    public RawImage rawImage;
    [Header("Texture")]
    public Texture redSign;
    public Texture yellowSign;
    public Texture redSignOffScreen;
    public Texture yellowSignOffScreen;
    public Texture noSign;
    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        cam = Camera.main;
        Vector3 p = cam.WorldToScreenPoint(alertSignTransform.position);  // p stands for the world to screen position

        if(p.z > 0 && p.x > 0 && p.x < Screen.width && p.y > 0 && p.y < Screen.height){  // target in front of player
            if(ms.alert && mv.timeNotSeen < 0.5f){  // if mech alert and can see target, red sign
                rawImage.texture = redSign;
            }else if(ms.alert && mv.timeNotSeen >= 0.5f){  // if the target is not directly visible, red sign
                rawImage.texture = yellowSign;
            }else{  // mech not alert, no sign
                rawImage.texture = noSign;
            }
            if(p != rectTransform.position){
                rectTransform.position = p;  // place UI
            }
            rectTransform.rotation = Quaternion.identity;
        }else{  // target behind player, use offscreen indicator instead
            rawImage.texture = noSign;


            Vector3 centre = new Vector3(Screen.width, Screen.height, 0) / 2;
            p -= centre;  // making the centre of the screen (0, 0)
            
            if(p.z < 0){
                p *= -1;  // mirror the whole world to screen position since it is behind the camera;
            }

            float angle = Mathf.Atan2(p.y, p.x);
            Vector3 bounds = centre * 0.9f;
            float m = Mathf.Tan(angle);

            if(p.x > 0){
                p = new Vector3(bounds.x, bounds.x * m, 0);
            }else{
                p = new Vector3(-bounds.x, -bounds.x * m, 0);
            }

            if(p.y > bounds.y){
                p = new Vector3(bounds.y / m, bounds.y, 0);
            }else if(p.y < -bounds.y){
                p = new Vector3(-bounds.y / m, -bounds.y, 0);
            }

            p += centre;

            // draw triangle
            if(ms.alert && mv.timeNotSeen < 0.5f){  // if mech alert and can see target, red sign
                rawImage.texture = redSignOffScreen;
            }else if(ms.alert && mv.timeNotSeen >= 0.5f){  // if the target is not directly visible, yellow sign
                rawImage.texture = yellowSignOffScreen;
            }else{  // mech not alert, no sign
                rawImage.texture = noSign;
            }
            rectTransform.position = p;
            angle -= 90 * Mathf.Deg2Rad;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
    }
}
