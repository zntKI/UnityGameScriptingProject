using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance => instance;
    static TimeManager instance;

    public static TimePhase TimePhase => instance.timePhase;
    TimePhase timePhase = TimePhase.Early;

    public static event Action OnTimePhaseChange;

    public static event Action OnTimePhaseChangeToMid;
    public static event Action OnTimePhaseChangeToEnd;
    public static event Action OnTimePhaseChangeToGameOver;

    public static event Action<int> OnMinutePassed;

    [SerializeField]
    int secondsForOneMinute;

    float timeCounter;
    int minPhaseCounter; // 'Minutes' since the current state started

    int minCounter; // 'Minutes' since the game started - used for updating UI

    [SerializeField]
    int phaseTimeMinutes;

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
        OnMinutePassed?.Invoke(minCounter);
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
                OnMinutePassed?.Invoke(minCounter);

                minPhaseCounter++;
                if (minPhaseCounter >= phaseTimeMinutes)
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
        OnTimePhaseChange?.Invoke();

        minPhaseCounter = 0;

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