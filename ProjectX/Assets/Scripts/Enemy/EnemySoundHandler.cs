using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundHandler : MonoBehaviour
{
    [Header("SFX")]
    [Space]

    [SerializeField]
    AudioSource audioSourceSFX;
    [Space]

    [SerializeField]
    AudioClip enemyHitWhilePatrolingSound;
    [SerializeField]
    AudioClip enemyHitWhileTargetingSound;

    void Start()
    {
    }

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
}
