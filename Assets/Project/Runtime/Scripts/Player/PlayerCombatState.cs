using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatState : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private PistolBehaviour pb;
    [SerializeField] private PlayerFinisher pf;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAudio playerAudio;
    private Timer timer;


    [Header("Health")]
    public float maxHealth;
    [HideInInspector] public float health;
    [SerializeField] private float regainHealthTime;
    [SerializeField] private float regainHealthSpeed;
    [HideInInspector] public float damageTimer;
    [HideInInspector] public bool dead;


    // pistol
    [HideInInspector] public int pistolBulletNum;
    [HideInInspector] public bool reloading;
    [HideInInspector] public float currentReloadTime;
    // finisher
    [HideInInspector] public int finisherType;
    [HideInInspector] public float finisherTime;
    [HideInInspector] public GameObject finisherTargetEnemy;

    // animation
    [HideInInspector] public string animationName;
    [HideInInspector] public float animationTime;

    // Start is called before the first frame update

    // Update is called once per frame
    void Start()
    {
        timer = GameObject.Find("Time").GetComponent<Timer>();
        dead = false;
    }

    void FixedUpdate()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        pistolBulletNum = pb.bulletNum;
        reloading = pb.reloading;
        currentReloadTime = pb.currentReloadTime;
        finisherType = pf.finisherType;
        finisherTime = pf.finisherTime;
        finisherTargetEnemy = pf.targetEnemy;

        animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (health <= 0 && !dead)
        {
            dead = true;
            GameObject.Find("Player").SendMessage("Death");  // broad casting to main player and end the game
            GameObject.Find("Player/Armature").GetComponent<CapsuleCollider>().enabled = false;  // disable player collider so no more environmental interactions
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("PlayerKilled");
        }

        // regain health
        if (!timer.rewinding)
        {
            damageTimer += Time.fixedDeltaTime;
        }

        if (damageTimer >= regainHealthTime && health < maxHealth)
        {
            health += regainHealthSpeed * Time.fixedDeltaTime;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    // message from enemy bullet
    public void ReduceHealth(float damage)
    {
        if (pf.finisherType != 0)
        {
            return;
        }

        health -= damage;
        damageTimer = 0;
        if (gameObject.name == "Player")
        {
            GameObject.Find("Environment/Lightings/post processing volume").SendMessage("PlayerDamaged");
            playerAudio.PlayDamagedAudio();
        }
    }
}
