using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance => instance;
    static UIManager instance;

    public static event Action OnOverlayOpened;
    public static event Action OnOverlayClosed;


    [Header("Common")]

    [SerializeField]
    Transform commonContainer;
    [SerializeField]
    TextMeshProUGUI timeText;
    [SerializeField]
    TextMeshProUGUI knifeText;
    [SerializeField]
    TextMeshProUGUI interactableText;


    [Header("Note")]

    [SerializeField]
    Transform noteOverlay;
    [SerializeField]
    TextMeshProUGUI noteText;
    [SerializeField]
    TextMeshProUGUI noteCloseText;


    [Header("Pause")]

    [SerializeField]
    Transform pauseMenu;

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

        InventoryManager.OnNotePickedUp += ShowNoteOverlay;
        InputHandler.OnNoteOverlayClose += CloseNoteOverlay;

        InputHandler.OnInteractionTextEnable += EnableInteractableText;
        InputHandler.OnInteractionTextDisable += DisableInteractableText;

        InputHandler.OnPauseMenuOpen += ShowPauseMenu;
    }

    void Start()
    {
        ShowNoteOverlay(noteText.text);
        //OnNoteOverlayClosed?.Invoke(); // ONLY for DEBUG - REMOVE for RELEASE

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
            //Debug.Log("Enabled interactable text");

            interactableText.text = $"{message} ({inputValue})";
        }
    }

    void DisableInteractableText()
    {
        if (interactableText.text != "")
        {
            //Debug.Log("Disabled interactable text");
            interactableText.text = "";
        }
    }

    void ShowNoteOverlay(string noteMessage)
    {
        Debug.Log("Open note overlay");
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child == noteOverlay)
            {
                child.gameObject.SetActive(true);
                noteText.text = noteMessage;
                noteCloseText.text = $"Close ({(KeyCode)InputValues.CloseNoteOverlay})";
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        // Pause time
        OnOverlayOpened?.Invoke();
    }

    void CloseNoteOverlay()
    {
        if (noteOverlay.gameObject.activeSelf)
        {
            Debug.Log("Close note overlay");
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child == commonContainer)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            // Resume time
            OnOverlayClosed?.Invoke();
        }
    }

    void ShowPauseMenu()
    {
        if (!pauseMenu.gameObject.activeSelf)
        {
            Debug.Log("Open pause menu");
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child == pauseMenu)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            // Pause time
            OnOverlayOpened?.Invoke();
        }
    }

    public void ClosePauseMenu()
    {
        Debug.Log("Close pause menu");
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child == commonContainer)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        Cursor.lockState = CursorLockMode.Locked;

        // Resume time
        OnOverlayClosed?.Invoke();
    }

    void OnDestroy()
    {
        TimeManager.OnMinutePassed -= UpdateTime;

        InventoryManager.OnKnifePickedUp -= UpdateKnifeCount;
        InventoryManager.OnThrownKnife -= UpdateKnifeCount;

        InventoryManager.OnNotePickedUp -= ShowNoteOverlay;
        InputHandler.OnNoteOverlayClose -= CloseNoteOverlay;

        InputHandler.OnInteractionTextEnable -= EnableInteractableText;
        InputHandler.OnInteractionTextDisable -= DisableInteractableText;

        InputHandler.OnPauseMenuOpen -= ShowPauseMenu;
    }
}
