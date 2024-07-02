using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip startBgMusic;
    [SerializeField]
    AudioClip midBgMusic;
    [SerializeField]
    AudioClip endBgMusic;
    [SerializeField]
    AudioClip gameOverBgMusic;

    [SerializeField]
    [Range(0f, 1f)]
    float startVolume = .6f;

    AudioSource audioSource;

    void Awake()
    {
        TimeManager.OnTimePhaseChangeToMid += ChangeBgMusicToMid;
        TimeManager.OnTimePhaseChangeToEnd += ChangeBgMusicToEnd;
        TimeManager.OnTimePhaseChangeToGameOver += ChangeBgMusicToGameOver;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = startBgMusic;
        audioSource.Play();
    }

    void ChangeBgMusicToMid()
    {
        SetupAudioSource(midBgMusic);

        audioSource.Play();
    }

    void ChangeBgMusicToEnd()
    {
        SetupAudioSource(endBgMusic);

        audioSource.Play();
    }

    void ChangeBgMusicToGameOver()
    {
        SetupAudioSource(gameOverBgMusic);

        audioSource.PlayDelayed(2);
    }

    void SetupAudioSource(AudioClip clipToPlayNext)
    {
        audioSource.Stop();
        audioSource.clip = clipToPlayNext;
        audioSource.volume = startVolume;
    }

    void Update()
    {
        audioSource.volume += Time.deltaTime / 10;
        Mathf.Clamp(audioSource.volume, 0f, 1f);
    }

    void OnDestroy()
    {
        TimeManager.OnTimePhaseChangeToMid -= ChangeBgMusicToMid;
        TimeManager.OnTimePhaseChangeToEnd -= ChangeBgMusicToEnd;
        TimeManager.OnTimePhaseChangeToGameOver -= ChangeBgMusicToGameOver;
    }
}
