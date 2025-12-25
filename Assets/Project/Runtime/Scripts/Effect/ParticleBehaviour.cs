using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    [HideInInspector] public float cntTime = 0;
    private Timer timer;
    private ParticleSystem particleSystem;
    [SerializeField] private ParticlesManager particlesManager;
    private PlayerControl playerControl;
    [HideInInspector] public float lifeTime = 0.3f;
    void Awake()
    {
        timer = GameObject.Find("Time").GetComponent<Timer>();
        particleSystem = GetComponent<ParticleSystem>();
        lifeTime = particleSystem.main.startLifetime.constant;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;
        if (timer.rewinding)
        {  // if rewinding
            cntTime -= Time.fixedDeltaTime;
            if (cntTime < 0)
            {
                Destroy(gameObject);
            }
            particleSystem.Simulate(cntTime);
        }
        else
        {
            if (particleSystem.isPaused)
            {
                particleSystem.Play();
            }
            cntTime += Time.fixedDeltaTime;
            if (cntTime >= lifeTime)
            {
                DestroyAndRecord();
            }
        }
    }

    void DestroyAndRecord()
    {
        // record in particles manager
        if (!timer.inPast)
        {
            particlesManager.destroyedParticlesList[particlesManager.destroyedParticlesList.Count - 1].Add(new DestroyedParticles(transform.position));
        }
        Destroy(gameObject);
    }
}
