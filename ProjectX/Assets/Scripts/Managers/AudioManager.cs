using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("BG Music")]
    [Space]

    [SerializeField]
    AudioSource audioSourceBG;
    [Space]

    [SerializeField]
    AudioClip startBgMusic;
    [SerializeField]
    AudioClip midBgMusic;
    [SerializeField]
    AudioClip endBgMusic;
    [SerializeField]
    AudioClip gameOverBgMusic;
    [SerializeField]
    AudioClip enemyTargetingBgMusic;

    [SerializeField]
    [Range(0f, 1f)]
    float startVolume = .6f;


    [Header("Pick up sfx")]
    [Space]

    [SerializeField]
    AudioSource audioSourceSFX;
    [Space]

    [SerializeField]
    AudioClip notePickUpSound;
    [SerializeField]
    AudioClip collapseNoteSound;
    [SerializeField]
    AudioClip knifePickUpSound;
    [SerializeField]
    AudioClip keyPickUpSound;
    [SerializeField]
    AudioClip knifeUnableToThrowSound;
    [Space]

    [SerializeField]
    AudioClip warningSound;

    void Awake()
    {
        TimeManager.OnTimePhaseChangeToMid += ChangeBgMusicToMid;
        TimeManager.OnTimePhaseChangeToEnd += ChangeBgMusicToEnd;
        TimeManager.OnTimePhaseChangeToGameOver += ChangeBgMusicToGameOver;

        InventoryManager.OnKeyPickedUp += PlayKeyPickUpSound;
        InventoryManager.OnNotePickedUp += PlayNotePickUpSound;
        InventoryManager.OnKnifePickedUp += PlayKnifePickUpSound;

        InventoryManager.OnUnableToThrowKnife += PlayUnableToThrowKnifeSound;

        InputHandler.OnNoteOverlayClose += PlayNoteCollapseSound;

        RandomEnemyMovement.OnEnemyTargeting += ChangeBgMusicToEnemyTargeting;
        RandomEnemyMovement.OnEnemyDistracted += PlayWarningSound;
    }

    void Start()
    {
        audioSourceBG.clip = startBgMusic;
        audioSourceBG.Play();
    }

    void PlayKnifePickUpSound(int obj)
    {
        audioSourceSFX.clip = knifePickUpSound;
        audioSourceSFX.Play();
    }

    void PlayNotePickUpSound(string obj)
    {
        audioSourceSFX.clip = notePickUpSound;
        audioSourceSFX.Play();
    }

    void PlayNoteCollapseSound()
    {
        audioSourceSFX.clip = collapseNoteSound;
        audioSourceSFX.Play();
    }

    void PlayKeyPickUpSound()
    {
        audioSourceSFX.clip = keyPickUpSound;
        audioSourceSFX.Play();
    }

    void PlayUnableToThrowKnifeSound()
    {
        audioSourceSFX.clip = knifeUnableToThrowSound;
        audioSourceSFX.Play();
    }

    void PlayWarningSound()
    {
        audioSourceSFX.clip = warningSound;
        audioSourceSFX.Play();
    }

    void ChangeBgMusicToMid()
    {
        SetupAudioSource(midBgMusic);

        audioSourceBG.Play();
    }

    void ChangeBgMusicToEnd()
    {
        SetupAudioSource(endBgMusic);

        audioSourceBG.Play();
    }

    void ChangeBgMusicToGameOver()
    {
        SetupAudioSource(gameOverBgMusic);

        audioSourceBG.PlayDelayed(2);
    }

    void ChangeBgMusicToEnemyTargeting()
    {
        SetupAudioSource(enemyTargetingBgMusic);

        audioSourceBG.Play();
    }

    void SetupAudioSource(AudioClip clipToPlayNext)
    {
        audioSourceBG.Stop();
        audioSourceBG.clip = clipToPlayNext;
        audioSourceBG.volume = startVolume;
    }

    void Update()
    {
        audioSourceBG.volume += Time.deltaTime / 10;
        Mathf.Clamp(audioSourceBG.volume, 0f, 1f);
    }

    void OnDestroy()
    {
        TimeManager.OnTimePhaseChangeToMid -= ChangeBgMusicToMid;
        TimeManager.OnTimePhaseChangeToEnd -= ChangeBgMusicToEnd;
        TimeManager.OnTimePhaseChangeToGameOver -= ChangeBgMusicToGameOver;

        InventoryManager.OnKeyPickedUp -= PlayKeyPickUpSound;
        InventoryManager.OnNotePickedUp -= PlayNotePickUpSound;
        InventoryManager.OnKnifePickedUp -= PlayKnifePickUpSound;

        InputHandler.OnNoteOverlayClose -= PlayNoteCollapseSound;
    }
}
