using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace ShadowShift.Enemy
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class UpperSpike : MonoBehaviour
    {
        [Tooltip("Since this enemy type has 2 main animations, Do and Undo, so we need a constant delay between these animations")]
        public float DelayBetweenDoAndUndo;
        [Tooltip("We need a one time start delay, so everything can start off smoothly")]
        public float StartDelay = 2.0f;

        private BoxCollider2D m_collider;

        public UnityEvent OnContactEnter;
        public UnityEvent OnContactExit;

        public bool ShouldAffectAnimationSpeed = false;
        public float AnimationSpeed = 1.0f;

        [Header("Randomization Scenario")]
        [Tooltip("If yes then we can use this var to assign a random animation speed to make it look more dynamic")]
        public bool ShouldAffectRandomSpeedImplementation = false;
        public float RandomMin = .5f;
        public float RandomMax = 3.0f;

        void Start()
        {
            Invoke(nameof(StartUpperSpikeBehaviour), StartDelay);
            m_collider = GetComponent<BoxCollider2D>();
            m_collider.isTrigger = true;

            if (ShouldAffectAnimationSpeed) GetComponent<Animator>().speed = this.AnimationSpeed;

            ApplyRandomAnimationSpeed();
        }

        public void ApplyRandomAnimationSpeed()
        {
            if (ShouldAffectRandomSpeedImplementation) GetComponent<Animator>().speed = Random.Range(RandomMin, RandomMax);
        }
        void StartUpperSpikeBehaviour()
        {
            StartCoroutine(UpperSpikeCoroutine());
        }
        IEnumerator UpperSpikeCoroutine()
        {
            while (true)
            {
                var animator = GetComponent<Animator>();
                animator.CrossFade("Do", .1f);
                yield return new WaitForSeconds(DelayBetweenDoAndUndo);
                animator.CrossFade("Undo", .1f);
            }
        }

        #region AnmiationEvents

        public void OnContactEnterMethod()
        {
            this.OnContactEnter?.Invoke();
        }
        public void OnContactExitMethod()
        {
            this.OnContactExit?.Invoke();
        }

        #endregion

    }

}