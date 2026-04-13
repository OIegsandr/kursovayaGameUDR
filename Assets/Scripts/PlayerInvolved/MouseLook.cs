using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity;
    public Transform playerBody;
    float xRotation = 0f; 

    void Start()
    {
        LockCursor();
    }
    void LateUpdate()
    {
        ApplyInputs();
    }

    private void ApplyInputs(){
        float mouseMoveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseMoveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; 

        xRotation -= mouseMoveY;
        xRotation = Math.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseMoveX);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

}
