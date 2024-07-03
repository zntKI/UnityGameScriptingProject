using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance => instance;
    static InputHandler instance;
    public static GameObject Player => instance.gameObject;

    Transform cameraTransform;

    public static event Action<GameObject> OnDoorInteraction;

    public static event Action<Transform> OnPickUp;

    public static event Action OnThrowKnife;

    public static event Action<string, KeyCode> OnInteractionTextEnable;
    public static event Action OnInteractionTextDisable;

    public static event Action OnNoteOverlayClose;

    public static event Action OnPauseMenuOpen;

    [SerializeField]
    float openDoorRayMaxDist = 4f;
    [SerializeField]
    float pickUpRayMaxDist = 2f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new InvalidOperationException("There can only be one Player in the scene, therefore only one InputHandler!");
        }
    }

    void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (cameraTransform == null)
            throw new InvalidOperationException("No main camera found!");
    }

    void Update()
    {
        switch (UIManager.State)
        {
            case UIState.HUD:

                RaycastHit hit;
                // Order of statements MATTERS (ray max dist)!
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, pickUpRayMaxDist)
                    && (hit.transform.CompareTag("Key")
                    || hit.transform.CompareTag("Note")
                    || hit.transform.CompareTag("Knife")))
                {
                    CheckForInteractables(hit.transform);
                }
                else if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, openDoorRayMaxDist)
                    && hit.transform.CompareTag("Door"))
                {
                    CheckDoorType(hit.transform);
                }
                else
                {
                    OnInteractionTextDisable?.Invoke();
                }

                if (Input.GetMouseButtonDown((int)InputValues.ThrowKnife))
                {
                    OnThrowKnife?.Invoke();
                }
                else if (Input.GetKeyDown((KeyCode)InputValues.OpenPauseMenu))
                {
                    OnPauseMenuOpen?.Invoke();
                }

                break;
            case UIState.NoteOverlay:

                if (Input.GetKeyDown((KeyCode)InputValues.CloseNoteOverlay))
                {
                    OnNoteOverlayClose?.Invoke();
                }

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Checks for input wheter the door needs a key to be opened or not
    /// </summary>
    void CheckDoorType(Transform obj)
    {
        DoorControl doorControl = obj.parent.GetComponent<DoorControl>();

        string interactableMessage = doorControl.IsOpen ? "Close" : "Open" + " door";
        OnInteractionTextEnable?.Invoke(interactableMessage, (KeyCode)InputValues.OpenDoor);

        if (Input.GetKeyDown((KeyCode)InputValues.OpenDoor))
        {
            OnDoorInteraction?.Invoke(doorControl.gameObject);
        }
    }

    void CheckForInteractables(Transform obj)
    {
        string interactableMessage = "";
        switch (obj.tag)
        {
            case "Key":
                interactableMessage = "Pick up Key";
                break;
            case "Note":
                interactableMessage = "Pick up Note";
                break;
            case "Knife":
                interactableMessage = "Pick up Knife";
                break;
            default:
                break;
        }
        OnInteractionTextEnable?.Invoke(interactableMessage, (KeyCode)InputValues.PickUp);

        if (Input.GetKeyDown((KeyCode)InputValues.PickUp))
        {
            OnPickUp?.Invoke(obj);
        }
    }
}

public enum InputValues
{
    PickUp = KeyCode.E,
    OpenDoor = KeyCode.Q,
    CloseNoteOverlay = KeyCode.R,
    OpenPauseMenu = KeyCode.Escape,
    ThrowKnife = 0
}