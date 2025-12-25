using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public GameObject assignedObject;
    public AudioClip clip;
    public AudioMixerGroup outPut;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    [Range(0f, 1f)]
    public float spacialBlend;
    public bool loop;
    [HideInInspector]
    public AudioSource source;
}
