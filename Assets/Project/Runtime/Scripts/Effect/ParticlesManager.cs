using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    [HideInInspector]
    public List<List<DestroyedParticles>> destroyedParticlesList;
    private Timer timer;
    private bool firstFrameAfter = false;
    [SerializeField] private GameObject particle;  // instance of particle
    private PlayerControl playerControl;

    // Start is called before the first frame update
    void Start()
    {
        destroyedParticlesList = new List<List<DestroyedParticles>>();
        timer = GameObject.Find("Time").GetComponent<Timer>();
    }

    void FixedUpdate()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerInput>().playerControl;
        if (timer.rewinding && destroyedParticlesList.Count > 0)
        {
            foreach (DestroyedParticles destroyedParticle in destroyedParticlesList[destroyedParticlesList.Count - 1])
            {
                GameObject p = Instantiate(particle, destroyedParticle.position, Quaternion.identity);
                p.SetActive(true);
                p.GetComponent<ParticleBehaviour>().cntTime = p.GetComponent<ParticleBehaviour>().lifeTime;
            }
            destroyedParticlesList.RemoveAt(destroyedParticlesList.Count - 1);
            firstFrameAfter = true;
        }
        else
        {
            if (firstFrameAfter)
            {
                destroyedParticlesList.Clear();
                firstFrameAfter = false;
            }
            destroyedParticlesList.Add(new List<DestroyedParticles>());
            if (destroyedParticlesList.Count > timer.maximumRecordingTime / Time.fixedDeltaTime)
            {
                destroyedParticlesList.RemoveAt(0);
            }
        }
    }
}
