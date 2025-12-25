using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlSpeech : TimeControlParent
{
    [SerializeField] private AudioClip[] zeroToNineClips;
    private GameManaging gameManager;
    private AudioSource numbersAudio;
    private bool speaking;
    private int currentPosition; // record the current number being spoken
    private List<int> pitList = new List<int>();

    protected override void StartInit()
    {
        numbersAudio = GetComponent<AudioSource>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManaging>();
    }

    protected override void Rewinding()
    {
        int pit = pitList[pitList.Count - 1];
        if (pit == -1)
        {
            speaking = false;
        }
        else
        {
            currentPosition = pit;
        }
        pitList.RemoveAt(pitList.Count - 1);
        firstFrameAfter = true;
    }

    protected override void NotRewinding()
    {
        if (pitList.Count > maxRecordingTime / Time.fixedDeltaTime)
        {
            pitList.RemoveAt(0);
        }
        if (firstFrameAfter)
        {
            firstFrameAfter = false;
            pitList.Clear();
        }

        // if is speaking
        if (speaking)
        {
            int nDeaths = gameManager.gameData.numberOfDeaths;
            if (!numbersAudio.isPlaying && nDeaths.ToString().Length - 1 >= currentPosition)
            {
                numbersAudio.clip = zeroToNineClips[(int)(nDeaths.ToString()[currentPosition] - '0')];
                currentPosition++;
                numbersAudio.Play();
            }
            else if (!numbersAudio.isPlaying && nDeaths.ToString().Length - 1 < currentPosition)
            {
                speaking = false;
            }
        }

        if (speaking)
        {
            pitList.Add(currentPosition);
        }
        else
        {
            pitList.Add(-1);
        }
    }

    // message from timeline tts numbers
    public void SpeakNumbers()
    {
        speaking = true;
        currentPosition = 0;
    }
}
