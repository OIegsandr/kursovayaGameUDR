using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController{
    public class MouseLook : MonoBehaviour{
        [SerializeField] private ScriptableStats _stats;
        public Transform playerBody;
        float xRotation = 0f; 

        void Start(){
            LockCursor();
        }
        
        void LateUpdate(){
            ApplyInputs();
        }

        private void ApplyInputs(){
            float mouseMoveX = Input.GetAxis("Mouse X") * _stats.Sensitivity * Time.deltaTime;
            float mouseMoveY = Input.GetAxis("Mouse Y") * _stats.Sensitivity * Time.deltaTime; 

            xRotation -= mouseMoveY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Поворот камеры вверх-вниз (X - вертикально)
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            
            // Поворот игрока влево-вправо (Y - горизонтально)
            playerBody.Rotate(Vector3.up * mouseMoveX);
        }

        private void LockCursor(){
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}
