using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    public float increaseAmount = 0.02f;
    public float maxSound = 1;
    public float time = .1f;
    AudioSource source;

    public bool setRandStart = false;
    public float randMin = 0;
    public float randMax = 5;
    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
        if (setRandStart == false)
        {
            InvokeRepeating("HandleSourceVolume", time, time);
            source.PlayDelayed(time);
        }
        else
        {
            float randSec = Random.Range(randMin, randMax);
            source.time = randSec;
            source.Play();
        }
    }

    void HandleSourceVolume()
    {
        if (source.volume <= maxSound)
            source.volume = source.volume + increaseAmount;
    }
}
