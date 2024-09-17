using System.Collections;
using UnityEngine;


namespace ShadowShift.Enemy
{
    /// <summary>
    /// Used for smooth serial appearance of spikes.
    /// </summary>
    public class SerialSpikes : MonoBehaviour
    {
        [SerializeField] float DelayBetweenEachSpike;
        [SerializeField] GameObject[] Spikes;
        [Tooltip("This is actually the total time, like when all the serial spikes are done appearing, then its time to start disappearing")]
        [SerializeField] float OverallRepititionTime = 5.0f;

        public bool IsAppearing = false;

        private void Start()
        {
            // first shutdown each spike via animation
            foreach (var spike in Spikes) spike.transform.GetChild(0).GetComponent<Animator>().CrossFade("Disappear", .1f);

            InvokeRepeating(nameof(ManageAppearingDisappearingState), OverallRepititionTime, OverallRepititionTime);
        }

        void ManageAppearingDisappearingState()
        {
            IsAppearing = !IsAppearing;

            if (IsAppearing) StartCoroutine(SpawnSerialSpikeCoroutine());
            else StartCoroutine(DespawnSerialSpikeCoroutine());
        }

        IEnumerator SpawnSerialSpikeCoroutine()
        {

            foreach (var spike in Spikes)
            {
                spike.transform.GetChild(0).GetComponent<Animator>().CrossFade("Appear", .1f);
                yield return new WaitForSeconds(DelayBetweenEachSpike);
            }
        }

        IEnumerator DespawnSerialSpikeCoroutine()
        {
            foreach (var spike in Spikes)
            {
                spike.transform.GetChild(0).GetComponent<Animator>().CrossFade("Disappear", .1f);
                yield return new WaitForSeconds(DelayBetweenEachSpike);
            }
        }
    }
}