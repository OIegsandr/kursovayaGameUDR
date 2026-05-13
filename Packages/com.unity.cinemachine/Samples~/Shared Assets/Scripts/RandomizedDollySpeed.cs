using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine.Samples
{
    [Serializable]
    public class RandomizedDollySpeed : SplineAutoDolly.ISplineAutoDolly
    {
        [Tooltip("Minimum speed the cart can travel")]
        public float MinSpeed = 2;
        [Tooltip("Maximum speed the cart can travel")]
        public float _stats.MaxSpeed = 10;
        [Tooltip("How quickly the cart can change speed")]
        public float Acceleration = 1;

        float m_Speed;
        float m_TargetSpeed;

        void SplineAutoDolly.ISplineAutoDolly.Validate() => _stats.MaxSpeed = Mathf.Max(_stats.MaxSpeed, MinSpeed);
        void SplineAutoDolly.ISplineAutoDolly.Reset() => m_Speed = m_TargetSpeed = (MinSpeed + _stats.MaxSpeed) / 2;
        bool SplineAutoDolly.ISplineAutoDolly.RequiresTrackingTarget => false;

        public float GetSplinePosition(
            MonoBehaviour sender, Transform target,
            SplineContainer spline, float currentPosition,
            PathIndexUnit positionUnits, float deltaTime)
        {
            if (Application.isPlaying && deltaTime > 0)
            {
                if (Mathf.Abs(m_Speed - m_TargetSpeed) < 0.01f)
                    m_TargetSpeed = UnityEngine.Random.Range(MinSpeed, _stats.MaxSpeed);
                if (m_Speed < m_TargetSpeed)
                    m_Speed = Mathf.Min(m_TargetSpeed, m_Speed + Acceleration * deltaTime);
                if (m_Speed > m_TargetSpeed)
                    m_Speed = Mathf.Max(m_TargetSpeed, m_Speed - Acceleration * deltaTime);

                return currentPosition + m_Speed * deltaTime;
            }
            return currentPosition;
        }
    }
}
