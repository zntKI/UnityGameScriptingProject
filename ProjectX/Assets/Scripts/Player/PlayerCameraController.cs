using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    Transform cameraToRotate;

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * GameManager.MouseSens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * GameManager.MouseSens;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraToRotate.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
