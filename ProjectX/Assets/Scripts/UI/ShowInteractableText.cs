using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowInteractableText : MonoBehaviour
{
    TextMeshProUGUI text;

    void Awake()
    {
        InputHandler.OnInteractionTextEnable += EnableInteractableText;
        InputHandler.OnInteractionTextDisable += DisableInteractableText;
    }

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        DisableInteractableText();
    }

    void EnableInteractableText(string message, KeyCode inputValue)
    {
        if (text.text == "")
        {
            Debug.Log("Enabled interactable text");

            text.text = $"{message} ({inputValue})";
        }
    }

    void DisableInteractableText()
    {
        if (text.text != "")
        {
            Debug.Log("Disabled interactable text");
            text.text = "";
        }
    }

    void OnDestroy()
    {
        InputHandler.OnInteractionTextEnable -= EnableInteractableText;
        InputHandler.OnInteractionTextDisable -= DisableInteractableText;
    }
}
