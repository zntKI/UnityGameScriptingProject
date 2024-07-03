using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance => instance;
    static InventoryManager instance;

    public static event Action OnKeyPickedUp;
    public static event Action<string> OnNotePickedUp;
    public static event Action<int> OnKnifePickedUp;

    public static event Action OnUnableToThrowKnife;
    public static event Action<int> OnThrownKnife;

    [SerializeField]
    GameObject knifePrefab;

    List<int> keys;
    List<string> storyNotes;
    int numOfKnives;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new InvalidOperationException("There can only be one TimeManager in the scene!");
        }

        keys = new List<int>();
        storyNotes = new List<string>();

        InputHandler.OnPickUp += HandlePickup;

        InputHandler.OnThrowKnife += ThrowKnife;
    }

    public static bool ContainsTheRightKey(int doorId)
        => instance.keys.Contains(doorId);

    void HandlePickup(Transform pickup)
    {
        switch (pickup.tag)
        {
            case "Key":
                var keyDoorComponent = pickup.GetComponent<KeyDoor>();
                if (keyDoorComponent == null)
                    throw new InvalidOperationException("Key must have a KeyDoor script component!");

                Debug.Log("Picked up key");
                keys.Add(keyDoorComponent.Id);

                OnKeyPickedUp?.Invoke();
                break;
            case "Note":
                var noteComponent = pickup.gameObject.GetComponent<Note>();
                if (noteComponent == null)
                    throw new InvalidOperationException("Note must have a Note script component!");

                Debug.Log("Picked up story note");
                storyNotes.Add(noteComponent.Text);

                OnNotePickedUp?.Invoke(noteComponent.Text);
                break;
            case "Knife":
                Debug.Log("Picked up knife");
                numOfKnives++;

                OnKnifePickedUp?.Invoke(numOfKnives);
                break;
            default:
                return;
        }

        Destroy(pickup.gameObject);
    }

    void ThrowKnife()
    {
        if (numOfKnives == 0)
        {
            Debug.Log("Do not have a knife to throw");

            OnUnableToThrowKnife?.Invoke();
            return;
        }

        Debug.Log("Threw a knife");
        numOfKnives--;

        // Spawn a knife
        Instantiate(knifePrefab, InputHandler.Player.transform.position + InputHandler.Player.transform.forward * 2f, InputHandler.Player.transform.rotation);

        OnThrownKnife?.Invoke(numOfKnives);
    }

    void OnDestroy()
    {
        InputHandler.OnPickUp -= HandlePickup;

        InputHandler.OnThrowKnife -= ThrowKnife;
    }
}
