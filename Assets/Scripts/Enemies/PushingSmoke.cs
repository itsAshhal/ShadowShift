using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace ShadowShift.Enemy
{
    /// <summary>
    /// The script refers to handle the pushing smoke which disturbs the movement of the mainPlayer.
    /// We need to increase its length and other such settings at both editor time and runtime.
    /// </summary>
    /// 
    [ExecuteAlways]
    public class PushingSmoke : MonoBehaviour
    {
        public enum ForceDirection { FromTop, FromBottom, FromRight, FromLeft }
        [Tooltip("Set this so we can use this from the PlayerController in order to apply the appropriate force at specific direction")]
        public ForceDirection M_ForceDirection;
        private ParticleSystem m_currentParticleSystem;

        [SerializeField] float EmissionOffMinTime = 2.0f;
        [SerializeField] float EmissionOffMaxTime = 2.0f;

        [Tooltip("This is the max force which will be applied to the playerController, to disturb its movement")]
        public float MaxForce = 2.0f;

        [Header("Emission Events")]
        public UnityEvent OnEmissionEnabled;
        public UnityEvent OnEmissionDisabled;
        [Tooltip("If turned on then the emission will always be on and won't get turned off")]
        public bool StayActive = false;


        private void Start()
        {
            m_currentParticleSystem = GetComponent<ParticleSystem>();

            if (!StayActive) InvokeRepeating(nameof(ManageEmission), Random.Range(EmissionOffMinTime, EmissionOffMaxTime), Random.Range(EmissionOffMinTime, EmissionOffMaxTime));
            else
            {
                var emission = m_currentParticleSystem.emission;
                emission.enabled = true;
            }



        }

        void ManageEmission()
        {
            Debug.Log($"ManageEmission is called");
            var emission = m_currentParticleSystem.emission;
            emission.enabled = !emission.enabled;

            if (emission.enabled) OnEmissionEnabled?.Invoke();
            else OnEmissionDisabled?.Invoke();
        }

        private void OnParticleCollision(GameObject other)
        {
            Debug.Log($"Particle, Colliding with particles");
        }
        private void OnParticleTrigger()
        {
            Debug.Log($"Particle, Trigger with particles");
        }

    }

}