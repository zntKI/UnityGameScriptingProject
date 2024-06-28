using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    static GameManager instance;

    const string sceneStartManuName = "MenuScreen";
    const string scene1Name = "Level1";
    const string scene2Name = "Level2";
    const string sceneGameEndName = "GameEndScreen";

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
        }

        RandomEnemyMovement.OnPlayerCaught += PlayerDie;

        UIManager.OnOverlayOpened += PauseTime;
        UIManager.OnOverlayClosed += ResumeTime;

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

    void OnDestroy()
    {
        RandomEnemyMovement.OnPlayerCaught -= PlayerDie;

        UIManager.OnOverlayOpened -= PauseTime;
        UIManager.OnOverlayClosed -= ResumeTime;
    }
}
