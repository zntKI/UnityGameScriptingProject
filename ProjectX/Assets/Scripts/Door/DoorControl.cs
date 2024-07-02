using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public bool IsOpen => anim.GetBool("IsOpen");

    [SerializeField]
    AudioClip doorOpen;
    [SerializeField]
    AudioClip doorClose;
    [SerializeField]
    AudioClip doorUnlock;
    [SerializeField]
    AudioClip doorLocked;

    AudioSource audioSource;

    Animator anim;

    void Awake()
    {
        InputHandler.OnDoorInteraction += InteractWithDoor;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    void InteractWithDoor(GameObject doorPivot)
    {
        if (doorPivot == gameObject)
        {
            var keyDoorComponent = doorPivot.GetComponentInChildren<KeyDoor>();

            if (keyDoorComponent == null || !keyDoorComponent.enabled)
            {
                Debug.Log("No key door");

                // Open door
                // Play a sound
                HandleDoorInteraction();
            }
            else
            {
                Debug.Log("Key door");

                if (InventoryManager.ContainsTheRightKey(keyDoorComponent.Id))
                {
                    Debug.Log("Unlocked door");

                    UnlockDoor(keyDoorComponent);

                    Invoke(nameof(OpenDoorAfterUnlocking), doorUnlock.length);
                }
                else
                {
                    Debug.Log("No key for that door");

                    audioSource.clip = doorLocked;
                    audioSource.Play();
                }
            }
        }
    }

    /// <summary>
    /// Disable component for further interactions
    /// </summary>
    void UnlockDoor(KeyDoor keyDoorComponent)
    {
        keyDoorComponent.enabled = false;

        audioSource.clip = doorUnlock;
        audioSource.Play();
    }

    void OpenDoorAfterUnlocking()
    {
        HandleDoorInteraction();
    }

    public void HandleDoorInteraction()
    {
        anim.SetBool("IsOpen", !IsOpen);

        audioSource.clip = IsOpen ? doorClose : doorOpen;
        audioSource.Play();
    }

    void OnDestroy()
    {
        InputHandler.OnDoorInteraction -= InteractWithDoor;
    }
}
