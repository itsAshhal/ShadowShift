using UnityEngine;
using Cinemachine;
using System.Collections;


namespace ShadowShift.Fusion
{
    /// <summary>
    /// Controls all the cinematics happening in the lobby or the actual gameplay, mostly from RPC
    /// </summary>
    public class LobbyCinematicsController : MonoBehaviour
    {
        #region Properites
        public CinemachineVirtualCamera LobbyCamera;
        public static LobbyCinematicsController Instance;

        #endregion

        #region Methods

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
        }

        /// <summary>
        /// Shakes the camera common for every player in the lobby before the gameplay starts,
        /// this gives a haptic feedback with a shake that the game has been started
        /// </summary>
        public void ShakeCamera_GameStart()
        {
            // get the perlin component
            var multiChannelPerlin = LobbyCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // change the amplitude and the frequency
            StartCoroutine(ShakeCameraCoroutine(.25f, multiChannelPerlin, 6.0f, 0.35f));
        }
        IEnumerator ShakeCameraCoroutine(float duration, CinemachineBasicMultiChannelPerlin perlinComponent, float amp, float fre)
        {
            perlinComponent.m_AmplitudeGain = amp;
            perlinComponent.m_FrequencyGain = fre;
            yield return new WaitForSeconds(duration);
            perlinComponent.m_AmplitudeGain = 0.0f;
            perlinComponent.m_FrequencyGain = 0.0f;
        }

        #endregion
    }
}
