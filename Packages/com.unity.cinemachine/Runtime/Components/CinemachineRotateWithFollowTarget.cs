using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a CinemachineComponent in the Aim section of the component pipeline.
    /// Its job is to match the tOrientation of the Follow target.
    /// </summary>
    [AddComponentMenu("Cinemachine/Procedural/Rotation Control/Cinemachine Rotate With Follow Target")]
    [SaveDuringPlay]
    [DisallowMultipleComponent]
    [CameraPipeline(CinemachineCore.Stage.Aim)]
    [RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineRotateWithFollowTarget.html")]
    public class CinemachineRotateWithFollowTarget : CinemachineComponentBase
    {
        /// <summary>
        /// How much time it takes for the aim to catch up to the target's rotation
        /// </summary>
        [Tooltip("How much time it takes for the aim to catch up to the target's rotation")]
        public float Damping = 0;

        Quaternion m_PreviousReferencetOrientation = Quaternion.identity;

        /// <summary>True if component is enabled and has a Follow target defined</summary>
        public override bool IsValid { get => enabled && FollowTarget != null; }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Aim stage</summary>
        public override CinemachineCore.Stage Stage { get => CinemachineCore.Stage.Aim; }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime() => Damping;

        /// <summary>Orients the camera to match the Follow target's tOrientation</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Not used.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid)
                return;

            Quaternion dampedtOrientation = FollowTargetRotation;
            if (deltaTime >= 0)
            {
                float t = VirtualCamera.DetachedFollowTargetDamp(1, Damping, deltaTime);
                dampedtOrientation = Quaternion.Slerp(
                    m_PreviousReferencetOrientation, FollowTargetRotation, t);
            }
            m_PreviousReferencetOrientation = dampedtOrientation;
            curState.RawtOrientation = dampedtOrientation;
            curState.ReferenceUp = dampedtOrientation * Vector3.up;
        }
    }
}
