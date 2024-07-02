using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagerSoundHandler : MonoBehaviour
{
    [SerializeField]
    AudioSource timeTickSoundSource;
    [SerializeField]
    AudioSource timePhaseChangeSoundSource;

    void Awake()
    {
        TimeManager.OnMinutePassed += PlayTimeTickSound;

        TimeManager.OnTimePhaseChange += PlayTimePhaseChangeSound;
    }

    void PlayTimeTickSound(int obj)
    {
        timeTickSoundSource.Play();
    }

    void PlayTimePhaseChangeSound()
    {
        timePhaseChangeSoundSource.Play();
    }

    void OnDestroy()
    {
        TimeManager.OnMinutePassed -= PlayTimeTickSound;

        TimeManager.OnTimePhaseChange -= PlayTimePhaseChangeSound;
    }
}
