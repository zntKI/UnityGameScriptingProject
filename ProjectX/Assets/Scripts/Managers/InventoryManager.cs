using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance => instance;
    static InventoryManager instance;

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

        storyNotes = new List<string>();

        InputHandler.OnNotePickup += PickUpStoryNote;
        InputHandler.OnKnifePickup += PickUpKnife;
    }

    void PickUpStoryNote(string message)
    {
        Debug.Log("Picked up story note");
        storyNotes.Add(message);
        // TODO: Play a sound - Fire an event
        // TODO: Update UI - Fire an event
    }

    void PickUpKnife()
    {
        Debug.Log("Picked up knife");
        numOfKnives++;
        // TODO: Play a sound - Fire an event
        // TODO: Update UI - Fire an event
    }
}
