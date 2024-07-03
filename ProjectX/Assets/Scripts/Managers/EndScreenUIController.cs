using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenUIController : MonoBehaviour
{
    public static bool IsGameOver = false;

    [SerializeField]
    Transform gameWinOverlay;
    [SerializeField]
    Transform gameOverOverlay;

    void Start()
    {
        EnableOverlay();
    }

    void EnableOverlay()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child == (IsGameOver ? gameOverOverlay : gameWinOverlay))
            {
                child.gameObject.SetActive(true);
            }
            else if (child == (!IsGameOver ? gameOverOverlay : gameWinOverlay))
            {
                child.gameObject.SetActive(false);
            }
        }

        Cursor.lockState = CursorLockMode.None;
    }
}
