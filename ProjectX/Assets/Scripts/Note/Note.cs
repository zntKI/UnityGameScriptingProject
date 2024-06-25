using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public string Text => noteText;

    [SerializeField]
    [TextArea]
    string noteText;
}
