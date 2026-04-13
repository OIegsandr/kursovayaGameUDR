using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        [SerializeField] private Transform camDirection;
        public CharacterController controller;
        private FrameInput _frameInput;
        private Vector3 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        #region voids

        void Update()
        {
            GatherInput();
            ApplyMovement();
        }

        void FixedUpdate()
        {
            HandleDirection();
        }

        #region movement

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                //JumpLogic
            }
        }
        private void HandleDirection()
        {
            Vector2 move2D = _frameInput.Move;

            Vector3 camForward = camDirection.forward;
            camForward.y = 0f; camForward.Normalize();
            
            Vector3 camRight = camDirection.right;
            camRight.y = 0f; camRight.Normalize();

            Vector3 moveDirrection = (camRight * move2D.x + camForward * move2D.y);

            if (moveDirrection.sqrMagnitude == 0f)
            {
                float deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;

                Vector3 target = Vector3.zero;
                _frameVelocity = Vector3.MoveTowards(
                    _frameVelocity,
                    new Vector3(target.x, _frameVelocity.y, target.z),
                    deceleration * Time.fixedDeltaTime
                );
            }
            else
            {

                Vector3 targetXZ = moveDirrection.normalized * Mathf.Min(moveDirrection.magnitude * _stats.MaxSpeed, _stats.MaxSpeed);
                Vector3 target = new Vector3(targetXZ.x, _frameVelocity.y, targetXZ.z);
                float accel = _stats.Acceleration;
                _frameVelocity = Vector3.MoveTowards(
                    _frameVelocity,
                    target,
                    accel * Time.fixedDeltaTime
               );
            }
        }

        private void ApplyMovement() => controller.Move(_frameVelocity * Time.deltaTime);

        #endregion

        #region collisions
        private bool _grounded = true; //temp, before implementing jumping


        #endregion

        #endregion

    }

   public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}