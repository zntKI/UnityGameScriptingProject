using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static event Action OnDoorOpen;

    [SerializeField]
    float rayCastMaxDist = 2f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
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
        if (hit.collider.CompareTag("Door") && Input.GetKeyDown(KeyCode.E))
        {
            hit.transform.parent.GetComponent<DoorControl>().HandleDoorInteraction();
        }
    }
}
