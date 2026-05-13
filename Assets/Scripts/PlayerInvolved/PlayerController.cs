using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(CharacterController))]
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

        #region Gravity

        private float fTime;
        private bool bJumpToConsume;
        private bool bBufferedJumpUsable;
        private bool bEndedJumpEarly;
        private bool bCoyoteUsable;
        private float fTimeJumpWasPressed;
        private float fFrameLeftGrounded = float.MinValue;
        private bool bGrounded;
        private bool bHasBufferedJump => bBufferedJumpUsable && fTime < fTimeJumpWasPressed + _stats.JumpBuffer;
        private bool bCanUseCoyote => bCoyoteUsable && !bGrounded && fTime < fFrameLeftGrounded + _stats.CoyoteTime;

        #endregion

        #region voids

        void Awake(){
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }
        void Update()
        {
            fTime += Time.deltaTime;
            GatherInput();
        }

        void FixedUpdate()
        {
            HandleDirection();

            CheckCollisions();
            HandleJump();
            HandleGravity();

            ApplyMovement();
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
                if (_frameInput.JumpDown){
                    bJumpToConsume = true;
                    fTimeJumpWasPressed = fTime;
                }
            }
        }
        private void HandleDirection(){
            Vector2 move2D = _frameInput.Move;

            Vector3 camForward = camDirection.forward;
            camForward.y = 0f; camForward.Normalize();
            
            Vector3 camRight = camDirection.right;
            camRight.y = 0f; camRight.Normalize();

            Vector3 moveDirrection = (camRight * move2D.x + camForward * move2D.y);

            if (moveDirrection.sqrMagnitude == 0f){
                float fDeceleration = bGrounded ? _stats.GroundDeceleration : _stats.AirDeceleration;

                Vector3 target = Vector3.zero;
                _frameVelocity = Vector3.MoveTowards(
                    _frameVelocity,
                    new Vector3(target.x, _frameVelocity.y, target.z),
                    fDeceleration * Time.fixedDeltaTime
                );
            }
            else{

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
        private void CheckCollisions(){
            bool wasGrounded = bGrounded;

            bGrounded = controller.isGrounded;

            if (bGrounded){
                if (!wasGrounded){
                    bCoyoteUsable = true;
                    bBufferedJumpUsable = true;
                    bEndedJumpEarly = false;

                    GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
                }
            }
            else if (wasGrounded){
                fFrameLeftGrounded = fTime;
                GroundedChanged?.Invoke(false, 0);
            }
        }

        #region jumping
        private void HandleJump(){
            if (!bEndedJumpEarly && !bGrounded && !_frameInput.JumpHeld && _frameVelocity.y > 0) 
                bEndedJumpEarly = true;
            if (!bJumpToConsume && !bHasBufferedJump) 
                return;
            if (bGrounded || bCanUseCoyote) 
                ExecuteJump();

            bJumpToConsume = false;
        }

        private void ExecuteJump(){
            bEndedJumpEarly = false;
            fTimeJumpWasPressed = 0;
            bBufferedJumpUsable = false;
            bCoyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        #endregion
        private void HandleGravity(){
            if (bGrounded && _frameVelocity.y <= 0f){
                _frameVelocity.y = -2f;
            }
            else{
                var inAirGravity = _stats.FallAcceleration;
                
                if (bEndedJumpEarly && _frameVelocity.y > 0) 
                    inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                
                _frameVelocity.y = Mathf.MoveTowards(
                    _frameVelocity.y, 
                    -_stats.MaxFallSpeed, 
                    inAirGravity * Time.fixedDeltaTime
                );
            }
        }

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