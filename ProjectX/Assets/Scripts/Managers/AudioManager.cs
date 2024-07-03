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
        SetupBGAudioSource(startBgMusic);

        currentBgMusic = startBgMusic;
    }

    void PlayKnifePickUpSound(int obj)
    {
        SetupSFXAudioSource(knifePickUpSound);
    }

    void PlayNotePickUpSound(string obj)
    {
        SetupSFXAudioSource(notePickUpSound);
    }

    void PlayNoteCollapseSound()
    {
        SetupSFXAudioSource(collapseNoteSound);
    }

    void PlayKeyPickUpSound()
    {
        SetupSFXAudioSource(keyPickUpSound);
    }

    void PlayUnableToThrowKnifeSound()
    {
        SetupSFXAudioSource(knifeUnableToThrowSound);
    }

    void PlayWarningSound()
    {
        SetupSFXAudioSource(warningSound);
    }

    void SetupSFXAudioSource(AudioClip clip)
    {
        audioSourceSFX.clip = clip;
        audioSourceSFX.Play();
    }

    void ChangeBgMusicToMid()
    {
        SetupBGAudioSource(midBgMusic);

        currentBgMusic = midBgMusic;
    }

    void ChangeBgMusicToEnd()
    {
        SetupBGAudioSource(endBgMusic);

        currentBgMusic = endBgMusic;
    }

    void ChangeBgMusicToGameOver()
    {
        SetupBGAudioSource(gameOverBgMusic, 2f);

        currentBgMusic = gameOverBgMusic;
    }

    void ChangeBgMusicToEnemyTargeting()
    {
        SetupBGAudioSource(enemyTargetingBgMusic);
    }

    void ChangeBackBgMusic()
    {
        SetupBGAudioSource(currentBgMusic);
    }

    void SetupBGAudioSource(AudioClip clipToPlayNext, float delay=0f)
    {
        audioSourceBG.Stop();
        audioSourceBG.clip = clipToPlayNext;
        audioSourceBG.volume = startVolume;

        audioSourceBG.PlayDelayed(delay);
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
