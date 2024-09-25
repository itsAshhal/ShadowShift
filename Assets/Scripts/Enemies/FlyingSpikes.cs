using System.Collections;
using System.Collections.Generic;
using ShadowShift.Player;
using UnityEngine;

namespace ShadowShift
{
    public enum SlowMotionState { Slow, Default }
    public class FlyingSpikes : MonoBehaviour
    {
        public Transform leftTarget;
        public Transform rightTarget;

        public float SpeedMax = 12f;
        public float SpeedMin = 5f;
        public float speed = 5f;
        private Transform currentTarget;

        // Start is called before the first frame update
        void Start()
        {
            // Ensure the parent of targets is null at start
            if (leftTarget != null) leftTarget.parent = null;
            if (rightTarget != null) rightTarget.parent = null;

            // Start moving towards the right target initially
            currentTarget = rightTarget;

            speed = Random.Range(SpeedMin, SpeedMax);

            // subscribe to the OnPressSlowMo event of the GameplayController as well
            if (GameplayController.Instance == null) return;
            GameplayController.Instance.OnPressSlowMo += this.OnPressSlowMo_Method;
        }

        void OnPressSlowMo_Method(bool value)
        {
            if (value) SetSlowMotionState(SlowMotionState.Slow);
            else SetSlowMotionState(SlowMotionState.Default);
        }

        // Update is called once per frame
        void Update()
        {
            if (currentTarget != null)
            {
                MoveTowardsTarget();
            }
        }

        void MoveTowardsTarget()
        {
            // Move the game object towards the current target at the specified speed
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

            // Check if the game object has reached the current target
            if (Vector3.Distance(transform.position, currentTarget.position) < 0.001f)
            {
                // Toggle the target
                if (currentTarget == rightTarget)
                    currentTarget = leftTarget;
                else
                    currentTarget = rightTarget;
            }
        }

        public void SetSlowMotionState(SlowMotionState state)
        {
            this.speed = state == SlowMotionState.Slow ? 4f : Random.Range(SpeedMin, SpeedMax);
        }
    }
}
