using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechCombatState : MonoBehaviour
{
    [Header("Health Control")]
    [SerializeField] private float initHealth;
    [HideInInspector] public float health;
    [SerializeField] private ParticleSystem smoke;
    [Header("Player Detection")]
    [SerializeField] private MechView mechView;

    [Header("Dead?")]
    // whether the mech is destroyed
    [HideInInspector] public bool dead = false;
    [HideInInspector] public AudioSource countDownSound;
    [HideInInspector] public AudioSource explosionSound;
    [HideInInspector] public float secondsAfterFinished;
    public float explosionTime;
    [SerializeField] private ParticleSystem explosion;

    // Start is called before the first frame update
    void Start()
    {
        health = initHealth;
        smoke.Stop();
        countDownSound = GetComponents<AudioSource>()[2];
        explosionSound = GetComponents<AudioSource>()[3];
        secondsAfterFinished = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dead)
        {
            if (secondsAfterFinished >= 0)
            {
                secondsAfterFinished += Time.fixedDeltaTime;
            }

            if (secondsAfterFinished >= explosionTime && transform.Find("Holder").gameObject.activeInHierarchy)
            {
                explosionSound.time = 0;
                explosionSound.Play();
                countDownSound.Stop();
                explosion.Play();
                transform.Find("Holder").gameObject.SetActive(false);
            }
        }
    }

    // message from player bullet
    public void BeingShot(GameObject target)
    {
        mechView.setAlert(target);
        health -= 1;
        if (health <= 0 && !smoke.isPlaying)
        {
            // if health below 0
            smoke.Play();
        }
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            GetComponent<MechNavigation>().enabled = false;
            GetComponent<MechShoot>().mechSound[1].Stop();
            GetComponent<MechShoot>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.Find("Holder/Armature/Hip/Turret").GetComponent<MechRoutine>().enabled = false;
            transform.Find("Holder/Armature/Hip/Turret/view zone").GetComponent<MechView>().enabled = false;
        }
    }

    // message from finisher animation
    public void StartCountDown()
    {
        secondsAfterFinished = 0;
        countDownSound.time = 0;
        countDownSound.Play();
    }
}
