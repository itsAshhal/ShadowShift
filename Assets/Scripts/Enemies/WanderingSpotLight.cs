using ShadowShift.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShadowShift.Enemy
{
    public class WanderingSpotLight : MonoBehaviour
    {
        public UnityEvent OnSpottedBegan;
        public UnityEvent OnSpottedStay;
        public UnityEvent OnNormal;
        [SerializeField] Transform m_leftEndTransform;
        [SerializeField] Transform m_rightEndTransform;
        [SerializeField] float m_wanderingSpeed = 1f;
        [Tooltip("When reached this distance, the enemy will start moving towards to the other target and vice versa")]
        [SerializeField] float m_reachingDistanceThreshold = .1f;

        private Vector2 m_distanceFromMainPlayer = Vector2.zero;  // so we can use this distance for other purposes like when player is spotted
        private Transform m_currentTargetTransform;

        public enum StartingTarget
        {
            Left, Right
        }
        public StartingTarget M_StartingTarget;

        public enum PlayerSpotState
        {
            Spotted, Normal
        }
        public PlayerSpotState M_PlayerSpotState;
        private float m_spotTimer = 0f;
        [Tooltip("If the player gets spotted, then there are max this number of seconds, under those seconds, player can hide, if the time limit is reached, this enemy will start shooting balls towards the mainPlayer")]
        [SerializeField] float m_maxSpotTimeBeforeAttacking = 2f;
        [SerializeField] float m_x_thresholdForStoppingNearMainPlayer = 1f;
        [Tooltip("When the player has been spotted, the enemy will try to chase him as well with this speed")]
        [SerializeField] float m_chasingSpeed = 1f;
        [SerializeField] float m_rightRotationZAngle = 40f;
        [SerializeField] float m_leftRotationZAngle = -40f;
        [SerializeField] float m_rotationSpeed = 10f;
        [SerializeField] Animator m_leftEarAnim;
        [SerializeField] Animator m_rightEarAnim;
        [SerializeField] int m_totalAlertAnimations = 1;
        [Tooltip("Will throw shooting balls after every this seconds")]
        [SerializeField] float m_shootingBallsRate = .5f;
        private float m_shootTimer = 0f;
        private bool m_shootingCoroutineStarted = false;


        private void Start()
        {
            var mainPlayer = GameplayController.Instance.MainPlayer;
            m_distanceFromMainPlayer = this.transform.position - GameplayController.Instance.MainPlayer.transform.position;
            m_currentTargetTransform = M_StartingTarget == StartingTarget.Right ? m_rightEndTransform : m_leftEndTransform;
            M_PlayerSpotState = PlayerSpotState.Normal;

            // also make sure the targets are not children once the game is started
            m_leftEndTransform.SetParent(null);
            m_rightEndTransform.SetParent(null);

            Debug.Log($"Y distance at start is {m_distanceFromMainPlayer.y}");
        }

        private void Update()
        {
            Move();
            ChasePlayer();



            Debug.Log($"Y distance at update is {m_distanceFromMainPlayer.y}");
        }

        /// <summary>
        /// Simply move this Enemy
        /// </summary>
        void Move()
        {
            if (M_PlayerSpotState == PlayerSpotState.Normal)
            {
                // Move the enemy
                transform.position = Vector2.MoveTowards(transform.position, m_currentTargetTransform.position, m_wanderingSpeed * Time.deltaTime);

                // check if the currentTargetTransform has been reached?
                if (Vector2.Distance(transform.position, m_currentTargetTransform.position) <= m_reachingDistanceThreshold)
                {
                    m_currentTargetTransform = m_currentTargetTransform == m_leftEndTransform ? m_rightEndTransform : m_leftEndTransform;
                }
            }

            else
            {
                // now in this state the enemy has detected the player and needs to catch him
                // just make he rests for now and we'll deal with this mechanic later after previous mechanic is tested
            }


        }

        void ChasePlayer()
        {
            if (M_PlayerSpotState == PlayerSpotState.Spotted)
            {
                m_spotTimer += Time.deltaTime;

                // check if the player has hidden himself or not
                if (GameplayController.Instance.MainPlayer.M_PlayerHiddenState == PlayerController.PlayerHiddenState.Open)
                {
                    if (m_spotTimer >= m_maxSpotTimeBeforeAttacking)
                    {
                        // the enemy needs to shoot balls towards the mainPlayer
                        // also try to move the enemy as close to the player as u can, in x coordinate and not the y coordintate

                        var mainPlayer = GameplayController.Instance.MainPlayer;

                        Vector2 target = new Vector2(mainPlayer.transform.position.x - m_x_thresholdForStoppingNearMainPlayer, transform.position.y);

                        Debug.Log($"Chasing target, {target}");

                        transform.position = Vector2.MoveTowards(transform.position, target, m_chasingSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    M_PlayerSpotState = PlayerSpotState.Normal;
                    m_spotTimer = 0f;
                }
            }
        }

        void RotateEnemy(Transform target)
        {
            if (target == m_leftEndTransform)
            {
                // enemy is moving right
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, m_leftRotationZAngle);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_rotationSpeed);
            }
            else
            {
                // enemy is movin left
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, m_rightRotationZAngle);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_rotationSpeed);
            }
        }


        public void Alert(bool isAlert = true)
        {
            if (isAlert)
            {
                string alertStateName = Random.Range(1, m_totalAlertAnimations).ToString();
                m_leftEarAnim.CrossFade($"Alert_{alertStateName}", .1f);
                m_rightEarAnim.CrossFade($"Alert_{alertStateName}", .1f);
            }
            else
            {
                m_leftEarAnim.CrossFade($"Default", .1f);
                m_rightEarAnim.CrossFade($"Default", .1f);
            }
        }

        public void ShootBalls()
        {
            if (m_shootingCoroutineStarted == true) return;
            StartCoroutine(ShootBallCoroutine());
        }
        IEnumerator ShootBallCoroutine()
        {
            m_shootingCoroutineStarted = true;
            while (M_PlayerSpotState != PlayerSpotState.Normal)
            {
                // Shooting mechanism
                var newBall = Instantiate(EffectsController.Instance.ShootingBalls, transform.position, Quaternion.identity);

                newBall.TryGetComponent<ShootingBall>(out ShootingBall shootingBall);
                if (shootingBall == null) yield return null;

                // a little wait
                yield return new WaitForSeconds(m_shootingBallsRate);
            }
            m_shootingCoroutineStarted = false;
        }

        #region TriggerEvents

        public void OnTriggerEnter_SpotLight(Collider2D collider)
        {
            // when the main player comes in contact with this spot light
            // wait for sometime to give the user some moments to hide
            if (GameplayController.Instance.MainPlayer.M_PlayerHiddenState == PlayerController.PlayerHiddenState.Hidden) return;
            M_PlayerSpotState = PlayerSpotState.Spotted;
            OnSpottedBegan?.Invoke();
        }
        public void OnTriggerStay_SpotLight(Collider2D collider)
        {
            if (GameplayController.Instance.MainPlayer.M_PlayerHiddenState == PlayerController.PlayerHiddenState.Hidden) return;
            OnSpottedStay?.Invoke();
            M_PlayerSpotState = PlayerSpotState.Spotted;

            // ok so since at this stage we're targeting the main player, make sure to rotate towards it as well
            /*ChasePlayer();*/
            //RotateEnemy(collider.transform);
        }

        public void OnTriggerExit_SpotLight(Collider2D collider)
        {
            // since the player is out of the spot light, we need the enemy to stop shooting balls at him
            Invoke(nameof(ResetEnemyNormalState), 2f);  // so after 2 seconds, enemy will again keep wandering in the environment
            m_spotTimer = 0f;
            OnNormal?.Invoke();
        }

        void ResetEnemyNormalState()
        {
            M_PlayerSpotState = PlayerSpotState.Normal;
        }

        Vector2 GetDirection(Vector2 a, Vector2 b)
        {
            return (a - b).normalized;
        }
        Quaternion GetQuaternionDirection(Vector2 a, Vector2 b)
        {
            var dir = (a - b).normalized;
            return Quaternion.LookRotation(dir);

        }
    }

    #endregion



}
