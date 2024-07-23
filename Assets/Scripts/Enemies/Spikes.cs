using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift.Enemy
{
    public class Spikes : MonoBehaviour
    {
        public enum SpikeTypes
        {
            BottomSpike, TopSpike
        }
        public SpikeTypes M_SpikeTypes;

        void Awake()
        {
            m_bottomSpikeAnim = GetComponent<Animator>();

            m_currentStateName = m_bottomSpikeAppearAnimationStateName;
            if (M_SpikeTypes == SpikeTypes.BottomSpike) InvokeRepeating(nameof(BottomSpikeMechanics), m_spikeAppearDuration, m_spikeAppearDuration);
        }

        #region BottomSpikes

        [Tooltip("The name of the animation state which will be used for Crossfade")]
        [SerializeField] string m_bottomSpikeAppearAnimationStateName;
        [SerializeField] string m_bottomSpikeDisappearAnimationStateName;
        [Tooltip("So the spike will appear after this duration and disappear after same duration")]
        [SerializeField] float m_spikeAppearDuration = 1f;
        private string m_currentStateName = string.Empty;
        private Animator m_bottomSpikeAnim;



        void BottomSpikeMechanics()
        {
            // play the required animation
            m_bottomSpikeAnim.CrossFade($"{m_currentStateName}", .1f);

            // now change the animation state name
            m_currentStateName = m_currentStateName == m_bottomSpikeAppearAnimationStateName ? m_bottomSpikeDisappearAnimationStateName : m_bottomSpikeAppearAnimationStateName;
        }

        #endregion

    }

}