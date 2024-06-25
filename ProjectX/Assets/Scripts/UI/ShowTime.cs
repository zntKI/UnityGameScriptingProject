using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ShowTime : MonoBehaviour
{
    TextMeshProUGUI text;

    int startHour = 22;

    void Awake()
    {
        TimeManager.OnMinutePassed += UpdateTime;
    }

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void UpdateTime(int minutes)
    {
        StringBuilder time = new StringBuilder();

        int hour = (startHour + minutes / 60) % 24;
        time.Append($"{(hour < 10 ? 0 : "")}{hour}:");

        minutes %= 60;
        time.Append($"{(minutes < 10 ? 0 : "")}{minutes}");

        text.text = time.ToString();
    }
}
