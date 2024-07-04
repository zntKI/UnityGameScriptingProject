using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    static GameManager instance;

    public static float MouseSens => instance.mouseSens;
    [SerializeField]
    float mouseSens = 500f;

    [SerializeField]
    AudioMixer audioMixer;

    const string sceneStartManuName = "MenuScreen";
    const string scene1Name = "Level1";
    const string scene2Name = "Level2";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // just to be sure
        }

        RandomEnemyMovement.OnPlayerCaught += PlayerDie;

        UIManager.OnOverlayOpened += PauseTime;
        UIManager.OnOverlayClosed += ResumeTime;

        UIManager.OnMouseSensitivityChanged += ChangeMouseSens;
        UIManager.OnVolumeChanged += ChangeVolume;

        PlayerMovement.OnPlayerFinish += LoadNextScene;
    }

    public void LoadNextScene()
    {
        int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (sceneBuildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            sceneBuildIndex = 0;
        }
        
        SceneManager.LoadScene(sceneBuildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void PlayerDie()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case scene1Name:
                SceneManager.LoadScene(sceneStartManuName);
                Cursor.lockState = CursorLockMode.None;
                break;
            case scene2Name: // Load GameOverScene
                LoadNextScene();
                EndScreenUIController.IsGameOver = true;
                break;
            default:
                throw new InvalidOperationException("There is no gameplay scene with such name!");
        }
    }

    void PauseTime()
    {
        Debug.Log("Paused time");
        Time.timeScale = 0f;
    }

    void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    void ChangeMouseSens(float mouseSensValue)
    {
        mouseSens = mouseSensValue;
    }

    void ChangeVolume(string volumeParamName, float value)
    {
        audioMixer.SetFloat(volumeParamName, value);
    }

    void OnDestroy()
    {
        RandomEnemyMovement.OnPlayerCaught -= PlayerDie;

        UIManager.OnOverlayOpened -= PauseTime;
        UIManager.OnOverlayClosed -= ResumeTime;

        UIManager.OnMouseSensitivityChanged -= ChangeMouseSens;
        UIManager.OnVolumeChanged -= ChangeVolume;

        PlayerMovement.OnPlayerFinish -= LoadNextScene;
    }
}
