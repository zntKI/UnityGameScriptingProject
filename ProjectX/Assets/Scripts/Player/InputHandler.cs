using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance => instance;
    static InputHandler instance;
    public static GameObject Player => instance.gameObject;


    public static event Action OnDoorOpen;
    public static event Action<string> OnNotePickup;
    public static event Action OnKnifePickup;

    public static event Action OnThrowKnife;

    [SerializeField]
    float pickUpRayCastMaxDist = 2f;
    [SerializeField]
    float knifeThrowRayCastMaxDist = 6f;

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

    void Update()
    {
        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(transform.position, transform.forward, out hit, pickUpRayCastMaxDist)) // Interact with objects
        {
            CheckForInteractables(hit);
        }
        else if (Input.GetMouseButtonDown(0)) // Throw knives
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, knifeThrowRayCastMaxDist))
                OnThrowKnife?.Invoke();
            else
            {
                // TODO: Add an event that just plays a sound indicating that throwing is not possible
            }
        }
    }

    void CheckForInteractables(RaycastHit hit)
    {
        switch (hit.collider.tag)
        {
            case "Door":
                OnDoorOpen?.Invoke(); // Play a sound
                hit.transform.parent.GetComponent<DoorControl>().HandleDoorInteraction();
                break;
            case "Note":
                // TODO: Play a sound
                // TODO: Update UI
                OnNotePickup?.Invoke("TODO: change this text when you have the right UI component for the note game object");
                Destroy(hit.collider.gameObject);
                break;
            case "Knife":
                // TODO: Play a sound
                // TODO: Update UI
                OnKnifePickup?.Invoke();
                Destroy(hit.collider.gameObject);
                break;
            default:
                break;
        }
    }
}
