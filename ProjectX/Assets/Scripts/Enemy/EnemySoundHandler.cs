using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundHandler : MonoBehaviour
{
    [Header("Footsteps")]
    [Space]

    [SerializeField]
    AudioSource audioSourceFootsteps;
    [Space]

    [SerializeField]
    AudioClip[] footStepSounds;


    [Header("SFX")]
    [Space]

    [SerializeField]
    AudioSource audioSourceSFX;
    [Space]

    [SerializeField]
    AudioClip enemyHitWhilePatrolingSound;
    [SerializeField]
    AudioClip enemyHitWhileTargetingSound;

    public void PlayHitWhilePatrolingSound()
    {
        audioSourceSFX.clip = enemyHitWhilePatrolingSound;
        audioSourceSFX.Play();
    }

    public void PlayHitWhileTargetingSound()
    {
        audioSourceSFX.clip = enemyHitWhileTargetingSound;
        audioSourceSFX.Play();
    }

    public void PlayFootStepSound()
    {
        AudioClip footStepSound = footStepSounds[Random.Range(0, footStepSounds.Length)];
        audioSourceFootsteps.PlayOneShot(footStepSound);
    }
}
