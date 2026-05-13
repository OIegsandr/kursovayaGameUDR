using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu]
    public class ScriptableStats : ScriptableObject
    {
        [Header("INPUT")] [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
        public bool SnapInput = true;

        [Header("INPUT")] [Tooltip("Player's X and Y sensitivity. Used for camera rotation.")]
        public float Sensitivity = 800;

        [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
        public float HorizontalDeadZoneThreshold = 0.3f;

        [Tooltip("Minimum input required before a forward or backward is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
        public float VerticalDeadZoneThreshold = 0.3f;

        [Header("MOVEMENT")] [Tooltip("The top crouching movement speed")]
        public float CrouchSpeed = 6;
        
        [Tooltip("The top walking movement speed")]
        public float WalkSpeed = 10;

        [Tooltip("The top sprinting movement speed")]
        public float SprintSpeed = 20;

        [Tooltip("The player's capacity to gain speed")]
        public float Acceleration = 80;

        [Tooltip("The pace at which the player comes to a stop")]
        public float GroundDeceleration = 60;

        [Tooltip("Deceleration in air only after stopping input mid-air")]
        public float AirDeceleration = 110;

        [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
        public float GrounderDistance = 0.05f;

        [Header("JUMP")] [Tooltip("The immediate velocity applied when jumping")]
        public float JumpPower = 30;

        [Tooltip("The maximum falling movement speed")]
        public float MaxFallSpeed = 200;

        [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
        public float FallAcceleration = 110;

        [Tooltip("The gravity multiplier added when jump is released early")]
        public float JumpEndEarlyGravityModifier = 3;

        [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
        public float CoyoteTime = .15f;

        [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
        public float JumpBuffer = .3f;
    }
}