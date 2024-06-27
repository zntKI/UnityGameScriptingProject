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
        }

        RandomEnemyMovement.OnPlayerCaught += PlayerDie;

        UIManager.OnOverlayOpened += PauseTime;
        UIManager.OnOverlayClosed += ResumeTime;

        PlayerMovement.OnPlayerFinish += LoadNextScene;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
                SceneManager.LoadScene(scene1Name); // TODO: Change to start menu when available
                break;
            case scene2Name:
                SceneManager.LoadScene(scene2Name);
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
