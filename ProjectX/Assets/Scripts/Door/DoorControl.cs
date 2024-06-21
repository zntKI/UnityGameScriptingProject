using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public bool IsOpen => anim.GetBool("IsOpen");

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void HandleDoorInteraction()
    {
        //=> anim.SetBool("IsOpen", !IsOpen);

    }
}
