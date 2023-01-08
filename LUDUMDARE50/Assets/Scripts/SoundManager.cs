using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip menuMusic;
    public AudioClip loadingMusic;
    public AudioClip startRoundMusic;
    public AudioClip gameMusic;

    public AudioSource audioSource;

    public float volume;

    [HideInInspector]
    public bool haveToLoadingMusic = false;
    [HideInInspector]
    public bool haveToStartRoundMusic = false;
    [HideInInspector]
    public bool haveToGameMusic = false;
    [HideInInspector]
    public bool haveToMenuMusic = false;

    private void Awake()
    {
        Instance = this;
        audioSource.volume = volume;
    }

    private void Update()
    {
        if (haveToMenuMusic)
        {
            playMenuMusic();
        }
        else if (haveToLoadingMusic)
        {
            playLoadingMusic();
        }
        else if (haveToStartRoundMusic)
        {
            playStartRoundMusic();
        }
        else if (haveToGameMusic)
        {
            playGameMusic();
        }
        audioSource.volume = Mathf.Lerp(audioSource.volume, volume, 0.01f);
    }

    private void playMenuMusic()
    {
        if (!audioSource.isPlaying || audioSource.clip == gameMusic)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
    }

    public void playLoadingMusic()
    {
        if (!audioSource.isPlaying || audioSource.clip == menuMusic)
        {
            audioSource.clip = loadingMusic;
            audioSource.Play();
        }
    }

    public void playStartRoundMusic()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.clip = startRoundMusic;
            audioSource.Play();
            haveToGameMusic = true;
            haveToStartRoundMusic = false;
        }
    }

    public void playGameMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = gameMusic;
            audioSource.Play();
        }
    }
    
}
