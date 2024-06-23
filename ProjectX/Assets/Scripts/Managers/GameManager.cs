using System;
using System.Collections;
using System.Collections.Generic;
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
        }
        else
        {
            throw new InvalidOperationException("There can only be one TimeManager in the scene!");
        }

        RandomEnemyMovement.OnPlayerCaught += PlayerDie;
    }

    private void PlayerDie()
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

    void OnDestroy()
    {
        RandomEnemyMovement.OnPlayerCaught -= PlayerDie;
    }
}
