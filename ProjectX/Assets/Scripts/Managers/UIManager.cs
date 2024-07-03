using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance => instance;
    static UIManager instance;

    public static UIState State => instance.state;
    UIState state;

    public static event Action OnUIButtonClicked;
    public static event Action OnUISliderChanged;

    public static event Action OnOverlayOpened;
    public static event Action OnOverlayClosed;

    public static event Action<float> OnMouseSensitivityChanged;
    public static event Action<string, float> OnVolumeChanged;


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
    Transform pauseContainer;
    [SerializeField]
    Transform pauseMenu;
    [SerializeField]
    Transform settingsMenu;

    [SerializeField]
    Slider mouseSensSlider;
    [SerializeField]
    Slider masterVolSlider, musicVolSlider, sfxVolumeSlider;

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
        state = UIState.HUD;

        ShowNoteOverlay(noteText.text);
        OnOverlayClosed?.Invoke(); // So that the NavMeshAgent finds its path!!!

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
        state = UIState.NoteOverlay;

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
        state = UIState.HUD;

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
        state = UIState.PauseOverlay;

        if (!pauseContainer.gameObject.activeSelf && !noteOverlay.gameObject.activeSelf)
        {
            Debug.Log("Open pause menu");
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child == pauseContainer)
                {
                    child.gameObject.SetActive(true);
                    for (int j = 0; j < child.transform.childCount; j++)
                    {
                        var innerChild = child.transform.GetChild(j);
                        if (innerChild == pauseMenu)
                        {
                            innerChild.gameObject.SetActive(true);
                        }
                        else if (innerChild == settingsMenu)
                        {
                            innerChild.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            Cursor.lockState = CursorLockMode.None;

            // Pause time
            OnOverlayOpened?.Invoke();
        }
    }

    public void ClosePauseMenu()
    {
        state = UIState.HUD;

        OnUIButtonClicked?.Invoke();

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

    public void ShowControlsPage()
    {
        OnUIButtonClicked?.Invoke();

        Debug.Log("Show settings menu");
        for (int i = 0; i < pauseContainer.childCount; i++)
        {
            var child = pauseContainer.GetChild(i);
            if (child == settingsMenu)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void CloseControlsPage()
    {
        OnUIButtonClicked?.Invoke();

        Debug.Log("Close settings menu");
        for (int i = 0; i < pauseContainer.childCount; i++)
        {
            var child = pauseContainer.GetChild(i);
            if (child == settingsMenu)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void ChangeMouseSensitivity()
    {
        OnUISliderChanged?.Invoke();

        OnMouseSensitivityChanged?.Invoke(mouseSensSlider.value);
    }
    public void ChangeMasterVol()
    {
        OnUISliderChanged?.Invoke();

        OnVolumeChanged?.Invoke("MasterVol", masterVolSlider.value);
    }
    public void ChangeMusicVol()
    {
        OnUISliderChanged?.Invoke();

        OnVolumeChanged?.Invoke("MusicVol", musicVolSlider.value);
    }
    public void ChangeSfxVol()
    {
        OnUISliderChanged?.Invoke();

        OnVolumeChanged?.Invoke("SFXVol", sfxVolumeSlider.value);
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

public enum UIState
{
    HUD, NoteOverlay, PauseOverlay
}