using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance => instance;
    static AudioManager instance;

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

    AudioClip currentBgMusic;


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

    [Header("UI sfx")]
    [Space]

    [SerializeField]
    AudioClip btnClickSound;
    [SerializeField]
    AudioClip sliderChangeSound;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new InvalidOperationException("There can be only one AudioManager in the scene!");
        }

        TimeManager.OnTimePhaseChangeToMid += ChangeBgMusicToMid;
        TimeManager.OnTimePhaseChangeToEnd += ChangeBgMusicToEnd;
        TimeManager.OnTimePhaseChangeToGameOver += ChangeBgMusicToGameOver;

        InventoryManager.OnKeyPickedUp += PlayKeyPickUpSound;
        InventoryManager.OnNotePickedUp += PlayNotePickUpSound;
        InventoryManager.OnKnifePickedUp += PlayKnifePickUpSound;

        InventoryManager.OnUnableToThrowKnife += PlayUnableToThrowKnifeSound;

        InputHandler.OnNoteOverlayClose += PlayNoteCollapseSound;

        RandomEnemyMovement.OnEnemyTargeting += ChangeBgMusicToEnemyTargeting;
        RandomEnemyMovement.OnEnemyRetreating += ChangeBackBgMusic;
        RandomEnemyMovement.OnEnemyDistracted += PlayWarningSound;

        UIManager.OnUIButtonClicked += PlayButtonClickSound;
        UIManager.OnUISliderChanged += PlaySliderChangeSound;
    }

    void Start()
    {
        audioSourceBG.volume = startVolume;

        audioSourceBG.clip = startBgMusic;
        audioSourceBG.Play();

        currentBgMusic = startBgMusic;
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

        currentBgMusic = midBgMusic;
    }

    void ChangeBgMusicToEnd()
    {
        SetupAudioSource(endBgMusic);

        audioSourceBG.Play();

        currentBgMusic = endBgMusic;
    }

    void ChangeBgMusicToGameOver()
    {
        SetupAudioSource(gameOverBgMusic);

        audioSourceBG.PlayDelayed(2);

        currentBgMusic = gameOverBgMusic;
    }

    void ChangeBgMusicToEnemyTargeting()
    {
        SetupAudioSource(enemyTargetingBgMusic);

        audioSourceBG.Play();
    }

    void ChangeBackBgMusic()
    {
        SetupAudioSource(currentBgMusic);

        audioSourceBG.Play();
    }

    void SetupAudioSource(AudioClip clipToPlayNext)
    {
        audioSourceBG.Stop();
        audioSourceBG.clip = clipToPlayNext;
        audioSourceBG.volume = startVolume;
    }

    public void PlayButtonClickSound()
    {
        audioSourceSFX.clip = btnClickSound;
        audioSourceSFX.Play();
    }

    public void PlaySliderChangeSound()
    {
        if (!audioSourceSFX.isPlaying)
        {
            audioSourceSFX.clip = sliderChangeSound;
            audioSourceSFX.Play();
        }
    }

    void Update()
    {
        audioSourceBG.volume += Time.deltaTime / 100;
        audioSourceBG.volume = Mathf.Clamp(audioSourceBG.volume, 0f, 0.3f);
    }

    void OnDestroy()
    {
        TimeManager.OnTimePhaseChangeToMid -= ChangeBgMusicToMid;
        TimeManager.OnTimePhaseChangeToEnd -= ChangeBgMusicToEnd;
        TimeManager.OnTimePhaseChangeToGameOver -= ChangeBgMusicToGameOver;

        InventoryManager.OnKeyPickedUp -= PlayKeyPickUpSound;
        InventoryManager.OnNotePickedUp -= PlayNotePickUpSound;
        InventoryManager.OnKnifePickedUp -= PlayKnifePickUpSound;

        InventoryManager.OnUnableToThrowKnife -= PlayUnableToThrowKnifeSound;

        InputHandler.OnNoteOverlayClose -= PlayNoteCollapseSound;

        RandomEnemyMovement.OnEnemyTargeting -= ChangeBgMusicToEnemyTargeting;
        RandomEnemyMovement.OnEnemyRetreating -= ChangeBackBgMusic;
        RandomEnemyMovement.OnEnemyDistracted -= PlayWarningSound;

        UIManager.OnUIButtonClicked -= PlayButtonClickSound;
        UIManager.OnUISliderChanged -= PlaySliderChangeSound;
    }
}
