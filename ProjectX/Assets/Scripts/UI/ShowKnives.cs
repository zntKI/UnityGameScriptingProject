using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowKnives : MonoBehaviour
{
    TextMeshProUGUI text;

    void Awake()
    {
        InventoryManager.OnKnifePickedUp += UpdateKnifeCount;
        InventoryManager.OnThrownKnife += UpdateKnifeCount;
    }

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void UpdateKnifeCount(int count)
    {
        text.text = $"Knives: {count}";
    }

    void OnDestroy()
    {
        InventoryManager.OnKnifePickedUp -= UpdateKnifeCount;
        InventoryManager.OnThrownKnife += UpdateKnifeCount;
    }
}
