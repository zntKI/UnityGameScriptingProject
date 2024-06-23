using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static event Action OnDoorOpen;
    public static event Action<string> OnNotePickup;
    public static event Action OnKnifePickup;

    [SerializeField]
    float rayCastMaxDist = 2f;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayCastMaxDist))
        {
            CheckHit(hit);
        }
    }

    void CheckHit(RaycastHit hit)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (hit.collider.tag)
            {
                case "Door":
                    OnDoorOpen?.Invoke(); // Play a sound
                    hit.transform.parent.GetComponent<DoorControl>().HandleDoorInteraction();
                    break;
                case "Note":
                    OnNotePickup?.Invoke("TODO: change this text when you have the right UI component for the note game object");
                    Destroy(hit.collider.gameObject);
                    break;
                case "Knife":
                    OnKnifePickup?.Invoke();
                    Destroy(hit.collider.gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
