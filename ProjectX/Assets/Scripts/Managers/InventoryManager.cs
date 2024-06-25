using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance => instance;
    static InventoryManager instance;

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

        InputHandler.OnKeyPickup += PickUpKey;
        InputHandler.OnNotePickup += PickUpStoryNote;
        InputHandler.OnKnifePickup += PickUpKnife;

        InputHandler.OnThrowKnife += ThrowKnife;
    }

    public static bool ContainsTheRightKey(int doorId)
        => instance.keys.Contains(doorId);

    void PickUpKey(int keyId)
    {
        Debug.Log("Picked up key");
        keys.Add(keyId);
    }

    void PickUpStoryNote(string message)
    {
        Debug.Log("Picked up story note");
        storyNotes.Add(message);
    }

    void PickUpKnife()
    {
        Debug.Log("Picked up knife");
        numOfKnives++;
    }

    void ThrowKnife()
    {
        if (numOfKnives == 0)
        {
            Debug.Log("Do not have a knife to throw");
            return;
        }

        Debug.Log("Threw a knife");
        numOfKnives--;

        // Spawn a knife
        Instantiate(knifePrefab, InputHandler.Player.transform.position + InputHandler.Player.transform.forward * 2f, InputHandler.Player.transform.rotation);
    }

    void OnDestroy()
    {
        InputHandler.OnKeyPickup -= PickUpKey;
        InputHandler.OnNotePickup -= PickUpStoryNote;
        InputHandler.OnKnifePickup -= PickUpKnife;

        InputHandler.OnThrowKnife -= ThrowKnife;
    }
}
