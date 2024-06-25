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

    public static event Action OnDoorUnlock;
    public static event Action OnDoorLocked;
    public static event Action OnDoorOpen;

    public static event Action<int> OnKeyPickup;
    public static event Action<string> OnNotePickup;
    public static event Action OnKnifePickup;

    public static event Action OnThrowKnife;

    [SerializeField]
    float openDoorRayMaxDist = 4f;
    [SerializeField]
    float pickUpRayMaxDist = 2f;
    [SerializeField]
    float knifeThrowRayMaxDist = 6f;

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
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            var child = transform.parent.GetChild(i);
            if (child.CompareTag("MainCamera"))
            {
                cameraTransform = child;
            }
        }
        if (cameraTransform == null)
            throw new InvalidOperationException("No main camera, child of Player pivot, found!");
    }

    void Update()
    {
        //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * pickUpRayMaxDist, Color.blue);

        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.F) && Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, openDoorRayMaxDist)
            && hit.collider.CompareTag("Door")) // Interact with objects
        {
            CheckDoorType(hit.transform);
        }
        else if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, pickUpRayMaxDist))
        {
            CheckForInteractables(hit);
        }
        else if (Input.GetMouseButtonDown(0)) // Throw knives
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, knifeThrowRayMaxDist))
                OnThrowKnife?.Invoke();
            else
            {
                // TODO: Add an event that just plays a sound indicating that throwing is not possible
            }
        }
    }

    /// <summary>
    /// Checks wheter the door needs a key to be opened or not
    /// </summary>
    void CheckDoorType(Transform door)
    {
        var keyDoorComponent = door.GetComponent<KeyDoor>();

        if (keyDoorComponent == null)
        {
            Debug.Log("No key door");

            door.parent.GetComponent<DoorControl>().HandleDoorInteraction();
            OnDoorOpen?.Invoke(); // Play a sound
        }
        else
        {
            Debug.Log("Key door");

            if (InventoryManager.ContainsTheRightKey(keyDoorComponent.Id))
            {
                Debug.Log("Unlocked door");

                OnDoorUnlock?.Invoke(); // Play a sound
                door.parent.GetComponent<DoorControl>().HandleDoorInteraction(); // TODO: Maybe add the option to be able to wait for a few seconds so that the previous sound could be played
                OnDoorOpen?.Invoke(); // Play a sound
            }
            else
            {
                Debug.Log("No key for that door");

                OnDoorLocked?.Invoke(); // Play a sound
            }
        }
    }

    void CheckForInteractables(RaycastHit hit)
    {
        switch (hit.collider.tag)
        {
            case "Key":
                var keyDoorComponent = hit.transform.GetComponent<KeyDoor>();
                if (keyDoorComponent == null)
                    throw new InvalidOperationException("Key must have a KeyDoor script component!");

                OnKeyPickup?.Invoke(keyDoorComponent.Id);
                break;
            case "Note":
                var noteComponent = hit.collider.gameObject.GetComponent<Note>();
                if (noteComponent == null)
                    throw new InvalidOperationException("Note must have a Note script component!");

                OnNotePickup?.Invoke(noteComponent.Text);
                break;
            case "Knife":
                OnKnifePickup?.Invoke();
                break;
            default:
                return;
        }

        Destroy(hit.collider.gameObject);
    }
}
