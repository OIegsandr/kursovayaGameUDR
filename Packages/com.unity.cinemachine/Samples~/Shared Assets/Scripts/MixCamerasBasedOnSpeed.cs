using UnityEngine;

namespace Unity.Cinemachine.Samples
{
    [RequireComponent(typeof(CinemachineMixingCamera))]
    public class MixCamerasBasedOnSpeed : MonoBehaviour
    {
        [Tooltip("At and above this speed, the second camera is going to be in control completely.")]
        public float _stats.MaxSpeed;
        public Rigidbody Rigidbody;
        CinemachineMixingCamera m_Mixer;
        void Start()
        {
            m_Mixer = GetComponent<CinemachineMixingCamera>();
        }

        void OnValidate()
        {
            _stats.MaxSpeed = Mathf.Max(1, _stats.MaxSpeed);
        }

        void FixedUpdate()
        {
            if (Rigidbody == null)
                return;

#if UNITY_2023_3_OR_NEWER
            var t = Mathf.Clamp01(Rigidbody.linearVelocity.magnitude / _stats.MaxSpeed);
#else
            var t = Mathf.Clamp01(Rigidbody.velocity.magnitude / _stats.MaxSpeed);
#endif
            m_Mixer.Weight0 = 1 - t;
            m_Mixer.Weight1 = t;
        }
    }
}
