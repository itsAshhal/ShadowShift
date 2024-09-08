using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShadowShift.Enemy
{
    public class RockEnemy : MonoBehaviour
    {
        public enum MovementType
        {
            LeftRight,
            UpDown
        }
        [SerializeField] float StartingTimeMin = 1.0f;
        [SerializeField] float StartingTimeMax = 3.0f;

        [SerializeField] private MovementType movementType = MovementType.LeftRight; // Default to LeftRight

        public float speed = 2.0f;
        public float moveDistance = 3.0f;
        public float waitTime = 2.0f;
        private Vector3 startPos;
        private bool movingPositiveDirection = true; // Tracks the direction of movement
        private float moveTimer;
        private float waitTimer;
        private float originalSpeed; // To keep track of the original speed
        [SerializeField] private Animator rockAnimator;
        [SerializeField] bool CanStartMoving = true;

        void Start()
        {
            startPos = transform.position;
            moveTimer = moveDistance / speed;
            waitTimer = waitTime;
            originalSpeed = speed; // Initialize original speed

            CanStartMoving = false;
            Invoke(nameof(TimeBasedMoving), Random.Range(StartingTimeMin, StartingTimeMax));
        }

        void TimeBasedMoving()
        {
            CanStartMoving = true;
        }

        void Update()
        {
            if (CanStartMoving == false) return;
            if (moveTimer > 0)
            {
                // Calculate how much of the journey remains
                float remainingDistance = moveTimer / (moveDistance / originalSpeed);
                // Smoothly decrease speed as the enemy nears the end of the move distance
                speed = Mathf.Lerp(0.1f, originalSpeed, remainingDistance);

                // Move the enemy based on the selected movement type
                if (movementType == MovementType.LeftRight)
                {
                    MoveHorizontal();
                }
                else if (movementType == MovementType.UpDown)
                {
                    MoveVertical();
                }

                moveTimer -= Time.deltaTime;

                // Handle animations based on movement direction
                HandleAnimation();
            }
            else if (waitTimer > 0)
            {
                // Reset speed to original when waiting
                speed = originalSpeed;
                waitTimer -= Time.deltaTime;
            }
            else
            {
                // Change direction and reset timers
                movingPositiveDirection = !movingPositiveDirection;
                moveTimer = moveDistance / speed;
                waitTimer = waitTime;
            }
        }

        private void MoveHorizontal()
        {
            // Move left or right based on direction
            float step = speed * Time.deltaTime * (movingPositiveDirection ? 1 : -1);
            transform.Translate(step, 0, 0);
        }

        private void MoveVertical()
        {
            // Move up or down based on direction
            float step = speed * Time.deltaTime * (movingPositiveDirection ? 1 : -1);
            transform.Translate(0, step, 0);
        }

        private void HandleAnimation()
        {
            // Control animations based on movement direction
            if (movementType == MovementType.LeftRight)
            {
                if (movingPositiveDirection)
                {
                    rockAnimator.CrossFade("RotateRight", 0.1f); // Replace with your actual animation state names
                }
                else
                {
                    rockAnimator.CrossFade("RotateLeft", 0.1f); // Replace with your actual animation state names
                }
            }
            else if (movementType == MovementType.UpDown)
            {
                if (movingPositiveDirection)
                {
                    rockAnimator.CrossFade("RotateUp", 0.1f); // Replace with your actual animation state names
                }
                else
                {
                    rockAnimator.CrossFade("RotateDown", 0.1f); // Replace with your actual animation state names
                }
            }
        }
    }
}
