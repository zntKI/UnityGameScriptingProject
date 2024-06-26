using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance => instance;
    static UIManager instance;

    [SerializeField]
    TextMeshProUGUI timeText;
    [SerializeField]
    TextMeshProUGUI knifeText;
    [SerializeField]
    TextMeshProUGUI interactableText;

    const int startHour = 22;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new InvalidOperationException("There can only be only one UIManager in the scene!");
        }

        TimeManager.OnMinutePassed += UpdateTime;

        InventoryManager.OnKnifePickedUp += UpdateKnifeCount;
        InventoryManager.OnThrownKnife += UpdateKnifeCount;

        InputHandler.OnInteractionTextEnable += EnableInteractableText;
        InputHandler.OnInteractionTextDisable += DisableInteractableText;
        DisableInteractableText();
    }

    void UpdateTime(int minutes)
    {
        StringBuilder time = new StringBuilder();

        int hour = (startHour + minutes / 60) % 24;
        time.Append($"{(hour < 10 ? 0 : "")}{hour}:");

        minutes %= 60;
        time.Append($"{(minutes < 10 ? 0 : "")}{minutes}");

        timeText.text = time.ToString();
    }

    void UpdateKnifeCount(int count)
    {
        knifeText.text = $"Knives: {count}";
    }

    void EnableInteractableText(string message, KeyCode inputValue)
    {
        if (interactableText.text == "")
        {
            Debug.Log("Enabled interactable text");

            interactableText.text = $"{message} ({inputValue})";
        }
    }

    void DisableInteractableText()
    {
        if (interactableText.text != "")
        {
            Debug.Log("Disabled interactable text");
            interactableText.text = "";
        }
    }

    void OnDestroy()
    {
        TimeManager.OnMinutePassed -= UpdateTime;

        InventoryManager.OnKnifePickedUp -= UpdateKnifeCount;
        InventoryManager.OnThrownKnife -= UpdateKnifeCount;

        InputHandler.OnInteractionTextEnable -= EnableInteractableText;
        InputHandler.OnInteractionTextDisable -= DisableInteractableText;
    }
}
