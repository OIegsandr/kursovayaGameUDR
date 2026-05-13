#if !CINEMACHINE_NO_CM2_SUPPORT
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>CinemachineSameAsFollowTarget has been deprecated. Use CinemachineRotateWithFollowTarget instead.</summary>
    [Obsolete("CinemachineSameAsFollowTarget has been deprecated. Use CinemachineRotateWithFollowTarget instead")]
    [SaveDuringPlay]
    [DisallowMultipleComponent]
    [CameraPipeline(CinemachineCore.Stage.Aim)]
    [RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineSameAsFollowTarget.html")]
    public class CinemachineSameAsFollowTarget : CinemachineComponentBase
    {
        /// <summary>
        /// How much time it takes for the aim to catch up to the target's rotation
        /// </summary>
        [Tooltip("How much time it takes for the aim to catch up to the target's rotation")]
        [FormerlySerializedAs("m_AngularDamping")]
        [FormerlySerializedAs("m_Damping")]
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

        // Helper to upgrade to CM3
        internal void UpgradeToCm3(CinemachineRotateWithFollowTarget c)
        {
            c.Damping = Damping;
        }
    }
}
#endif
