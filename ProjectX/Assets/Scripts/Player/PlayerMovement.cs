using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action OnPlayerFinish;

    [SerializeField]
    float moveSpeed;

    Rigidbody rb;

    Vector3 moveInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = moveInput.z * transform.forward + moveInput.x * transform.right;
        rb.velocity = moveDirection * moveSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            OnPlayerFinish?.Invoke();
        }
    }
}
