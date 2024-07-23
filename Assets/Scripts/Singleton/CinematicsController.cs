using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace ShadowShift
{
    public enum ShakeLevel
    {
        Low, Medium, High
    }
    public class CinematicsController : Singleton<CinematicsController>
    {
        public CinemachineVirtualCamera MainCamera;
        public float M_AmplitudeLow = 1f;
        public float M_AmplitudeMedium = 1f;
        public float M_AmplitudeHigh = 1f;
        public float M_Frequency = 3f;
        public float ShakeDuration = .25f;
        public ShakeLevel M_ShakeLevel;
        public void ApplyShake(float duration)
        {
            StartCoroutine(ShakeCoroutine(duration));
        }

        IEnumerator ShakeCoroutine(float duration)
        {
            var perlinChannel = MainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            float amplitude = 0f;
            if (M_ShakeLevel == ShakeLevel.Low) amplitude = M_AmplitudeLow;
            if (M_ShakeLevel == ShakeLevel.Medium) amplitude = M_AmplitudeMedium;
            if (M_ShakeLevel == ShakeLevel.High) amplitude = M_AmplitudeHigh;

            perlinChannel.m_AmplitudeGain = amplitude;
            perlinChannel.m_FrequencyGain = M_Frequency;

            yield return new WaitForSeconds(duration);

            perlinChannel.m_AmplitudeGain = 0f;
            perlinChannel.m_FrequencyGain = 0f;


        }
    }
}