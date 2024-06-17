using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource source;

    public AudioClip step;
    public AudioClip jumpStart;
    public AudioClip jumpEnd;
    public AudioClip attack;
    public AudioClip hit;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayStepSound()
    {
        source.clip = step;
        source.volume = 0.5f;
        source.Play();
    }

    public void PlayJumpStartSound()
    {
        source.clip = jumpStart;
        source.volume = 0.5f;
        source.Play();
    }

    public void PlayJumpEndSound()
    {
        source.clip = jumpEnd;
        source.volume = 0.5f;
        source.Play();
    }

    public void PlayAttackSound()
    {
        source.clip = attack;
        source.volume = 0.8f;
        source.Play();
    }

    public void PlayHitSound()
    {
        source.clip = hit;
        source.volume = 0.7f;
        source.Play();
    }


}
