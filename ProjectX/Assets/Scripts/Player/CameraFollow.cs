using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform player;

    Vector3 offset;

    void Start()
    {
        offset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + player.rotation * offset;
    }
}
