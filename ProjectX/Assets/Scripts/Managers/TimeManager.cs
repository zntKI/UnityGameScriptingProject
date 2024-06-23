using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance => instance;
    static TimeManager instance;

    TimePhase timePhase = TimePhase.Early;

    public static event Action OnTimePhaseChangeToMid;
    public static event Action OnTimePhaseChangeToEnd;
    public static event Action OnTimePhaseChangeToGameOver;

    [SerializeField]
    int secondsForOneMinute;

    float timeCounter;
    int minCounter;

    [SerializeField]
    int phaseTimeMin;

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
    }

    void Start()
    {
    }

    void Update()
    {
        HandleTimePhase();
    }

    void HandleTimePhase()
    {
        if (timePhase != TimePhase.GameOver)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= secondsForOneMinute)
            {
                minCounter++;
                if (minCounter > phaseTimeMin)
                    SetTimePhase();

                timeCounter = 0;
            }
        }
    }

    void SetTimePhase()
    {
        timePhase++;
        if (timePhase > TimePhase.GameOver)
        {
            throw new InvalidOperationException("Trying to increment timePhase above the last state!");
        }

        Debug.Log($"Changed time phase to: {timePhase}");

        minCounter = 0;

        // Fire events
        switch (timePhase)
        {
            case TimePhase.Mid:
                OnTimePhaseChangeToMid?.Invoke();
                break;
            case TimePhase.End:
                OnTimePhaseChangeToEnd?.Invoke();
                break;
            case TimePhase.GameOver:
                OnTimePhaseChangeToGameOver?.Invoke();
                break;
            default:
                throw new InvalidOperationException("Invalid timePhase value in event firing!");
        }
    }
}

public enum TimePhase
{
    Early, Mid, End, GameOver
}