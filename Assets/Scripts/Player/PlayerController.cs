using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using ShadowShift.DataModels;
using ShadowShift.Enemy;

namespace ShadowShift.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float m_moveSpeed;
        [SerializeField] float m_jumpSpeed;
        [SerializeField] bool m_playerCanRotateWhenNotTouchingTheGound = true;
        [SerializeField] float m_rotationSpeed = 2f;
        public bool m_canJump = true;

        private Rigidbody2D m_rb;
        [SerializeField] Animator m_anim;  // this is the main object, the black player as the child of this controller
        public SpriteRenderer MainSprite { get; set; }
        private Quaternion m_originalRotation;

        public enum PlayerHiddenState
        {
            Open, Hidden
        }
        public PlayerHiddenState M_PlayerHiddenState;



        #region Unity
        void Awake()
        {

        }

        private void Start()
        {
            //M_PlayerMovement = PlayerMovement.None;
            m_rb = GetComponent<Rigidbody2D>();
            //m_anim = GetComponent<Animator>();

            MainSprite = m_anim.GetComponent<SpriteRenderer>();


            // setting up input controls with actions
            InputController.Instance.OnMoveRight += OnMovePlayerRight;
            InputController.Instance.OnMoveLeft += OnMovePlayerLeft;
            InputController.Instance.OnNothing += OnMovePlayerNoWhere;
            InputController.Instance.OnPressJump += OnPlayerJump;

            m_originalRotation = transform.rotation;

            SetPlayerColor();
        }

        void SetPlayerColor()
        {
            // change the color of the player's main sprite as well
            var selectedColor = GameData.SelectedColor;
            selectedColor.a = 1.0f;
            MainSprite.color = selectedColor;
        }

        void Update()
        {
            Debug.Log($"TouchCount {Input.touchCount}");



            if (m_playerCanRotateWhenNotTouchingTheGound)
            {
                if (m_canJump == false)
                {
                    // it means we're in the air
                    float rotationAmount = m_rotationSpeed * Time.deltaTime;
                    Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAmount);
                    transform.rotation *= rotation;
                }
            }
        }

        void FixedUpdate()
        {
            if (InputController.Instance.M_PlayerMovement == InputController.PlayerMovement.Right) MovePlayerRight();
            if (InputController.Instance.M_PlayerMovement == InputController.PlayerMovement.Left) MovePlayerLeft();
        }

        void MovePlayerRight()
        {
            Debug.Log($"Actions working");
            Vector2 movementVector = Vector2.zero;
            movementVector = Vector2.right * m_moveSpeed;
            m_rb.velocity = new Vector2(movementVector.x, m_rb.velocity.y);

            if (m_canJump == false) return;  // we can only change static directions when we're at the ground
            transform.right = Vector2.right;
        }

        void MovePlayerLeft()
        {
            Debug.Log($"Actions working");
            Vector2 movementVector = Vector2.zero;
            movementVector = Vector2.left * m_moveSpeed;
            m_rb.velocity = new Vector2(movementVector.x, m_rb.velocity.y);

            if (m_canJump == false) return;  // we can only change static directions when we're at the ground
            transform.right = Vector2.left;
        }

        #endregion

        #region Control Actions

        private void OnPlayerJump(Touch touch)
        {
            Debug.Log($"Actions working");
            if (m_canJump == false) return;
            m_rb.velocity = new Vector2(m_rb.velocity.x, m_jumpSpeed);
        }

        private void OnMovePlayerNoWhere(Touch touch)
        {
            Debug.Log($"Actions working");
            m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
        }

        private void OnMovePlayerLeft(Touch touch)
        {
            transform.right = Vector2.left;
            return;
            Debug.Log($"Actions working");
            Vector2 movementVector = Vector2.zero;
            movementVector = Vector2.left * m_moveSpeed * Time.deltaTime;
            m_rb.velocity = new Vector2(movementVector.x, m_rb.velocity.y);

            if (m_canJump == false) return;  // we can only change static directions when we're at the ground
            transform.right = Vector2.left;
        }

        private void OnMovePlayerRight(Touch touch)
        {
            transform.right = Vector2.right;
            return;
            Debug.Log($"Actions working");
            Vector2 movementVector = Vector2.zero;
            movementVector = Vector2.right * m_moveSpeed * Time.deltaTime;
            m_rb.velocity = new Vector2(movementVector.x, m_rb.velocity.y);

            if (m_canJump == false) return;  // we can only change static directions when we're at the ground
            transform.right = Vector2.right;
        }

        #endregion

        #region Trigger Actions

        #region Ground

        [Header("Ground settings")]
        [SerializeField] LayerMask m_groundLayermask;

        public void OnEnterGround(Collider2D collider)
        {

            m_canJump = true;

            m_anim.SetBool("IsJumping", false);

            transform.rotation = m_originalRotation;

            // Play the OnTouchGround animation as well
            m_anim.CrossFade("OnTouchGround", .1f);

            // since when we touch the ground, make sure to retreat to the default original rotation as well
            // Getting reference to the default rotation
            if (InputController.Instance.M_PreviousMovement == InputController.PreviousMovement.Right) transform.right = Vector2.right;
            else if (InputController.Instance.M_PreviousMovement == InputController.PreviousMovement.Left) transform.right = Vector2.left;
        }
        public void OnStayGround(Collider2D collider)
        {
            m_canJump = true;

            //transform.rotation = m_originalRotation;


            return;
            if (InputController.Instance.M_PreviousMovement == InputController.PreviousMovement.Right) transform.right = Vector2.right;
            else if (InputController.Instance.M_PreviousMovement == InputController.PreviousMovement.Left) transform.right = Vector2.left;

            // Play the OnTouchGround animation as well
            //m_anim.CrossFade("OnTouchGround", .1f);

            // since when we touch the ground, make sure to retreat to the default original rotation as well
            // Getting reference to the default rotation
            // if (InputController.Instance.M_PreviousMovement == InputController.PreviousMovement.Right) transform.right = Vector2.right;
            // else if (InputController.Instance.M_PreviousMovement == InputController.PreviousMovement.Left) transform.right = Vector2.left;
        }
        public void OnExitGround(Collider2D collider)
        {
            m_canJump = false;
            m_anim.SetBool("IsJumping", true);

            Debug.Log($"OnExit called");

            // since we are not touching the ground here, we need to make sure we keep on rotating in update function
        }

        #endregion


        #region Hide
        [Header("Hide settings")]
        [SerializeField] float m_imageFadeDuration = .5f;
        [SerializeField] SpriteRenderer m_playerEyeSprite;

        public void Fade(SpriteRenderer spriteRenderer, bool fadeIn, float duration)
        {
            if (fadeIn)
            {
                spriteRenderer.DOFade(1f, duration); // Fade in to full opacity
            }
            else
            {
                spriteRenderer.DOFade(0f, duration); // Fade out to zero opacity
            }
        }

        public void OnEnter_Hide(Collider2D collider)
        {
            m_anim.SetBool("IsHiding", true);
            Fade(m_playerEyeSprite, false, m_imageFadeDuration);
            M_PlayerHiddenState = PlayerHiddenState.Hidden;
            m_canJump = true;
        }
        public void OnStay_Hide(Collider2D collider)
        {
            // since the jump is having issues so we need to keep the m_canJump=true as long as we are in contact with the Hidden elements
            m_canJump = true;
        }
        public void OnExit_Hide(Collider2D collider)
        {
            m_anim.SetBool("IsHiding", false);
            Fade(m_playerEyeSprite, true, m_imageFadeDuration);
            M_PlayerHiddenState = PlayerHiddenState.Open;
        }

        #endregion


        #region Enemy
        [Header("Enemy settings")]

        [SerializeField] Transform[] m_deathParticles;
        [SerializeField] float m_force = 2f;

        public void OnEnter_Enemy(Collider2D collider)
        {
            //m_deathParticles[0].transform.GetComponentInParent<Transform>().gameObject.SetActive(true);

            // spawn a random particle as well
            var part = EffectsController.Instance.DeathEffects[Random.Range(0, EffectsController.Instance.DeathEffects.Length)];
            EffectsController.Instance.SpawnParticle(transform.position, part, 2f);

            // Apply shake
            CinematicsController.Instance.ApplyShake(CinematicsController.Instance.ShakeDuration);

            foreach (var particle in m_deathParticles)
            {
                Vector2 direction = GameplayController.Instance.GetRandomDirection2D();
                particle.AddComponent<Rigidbody2D>().AddForce(direction * m_force, ForceMode2D.Impulse);

                particle.transform.SetParent(null);

                // destory it after some time
                Destroy(particle.gameObject, 1f);
            }

            // shut this player off
            this.gameObject.SetActive(false);
        }
        public void OnStay_Enemy(Collider2D collider)
        {

        }
        public void OnExit_Enemy(Collider2D collider)
        {

        }


        #endregion


        #region PushingSmoke
        public void OnEnter_PushingSmoke(Collider2D collider)
        {

        }
        public void OnStay_PushingSmoke(Collider2D collider)
        {
            // ok so here when we are in stay mode with this kind of trigger,
            // we need to make sure about the kind of force being applied to us



            // Get the direction var from the PushingSmoke Script
            collider.transform.parent.TryGetComponent<PushingSmoke>(out PushingSmoke pushingSmoke);
            if (pushingSmoke == null) return;

            Vector2 appliedForce = Vector2.zero;
            float maxForce = pushingSmoke.MaxForce;

            var forceDirection = pushingSmoke.M_ForceDirection;
            switch (forceDirection)
            {
                case PushingSmoke.ForceDirection.FromBottom:
                    appliedForce = Vector2.up;
                    break;

                case PushingSmoke.ForceDirection.FromTop:
                    appliedForce = Vector2.down;
                    break;

                case PushingSmoke.ForceDirection.FromRight:
                    appliedForce = Vector2.left;
                    break;

                case PushingSmoke.ForceDirection.FromLeft:
                    appliedForce = Vector2.right;
                    break;
            }

            Debug.Log($"Pushing Smoke On Stay Is Working, force is {appliedForce}");

            // Apply a smooth force over time using AddForce instead of setting velocity directly
            m_rb.AddForce(appliedForce * maxForce, ForceMode2D.Force); // ForceMode2D.Force applies a continuous force over time

        }
        public void OnExit_PushingSmoke(Collider2D collider)
        {

        }


        #endregion


        #region FlyingSpikeGround

        private float _playerX;


        public void OnEnter_FlyingSpikeGround(Collider2D collider)
        {
            transform.position = new Vector3(collider.transform.position.x, transform.position.y);


            m_canJump = true;

            m_anim.SetBool("IsJumping", false);

            _playerX = transform.position.x;

            transform.rotation = m_originalRotation;

            // Play the OnTouchGround animation as well
            m_anim.CrossFade("OnTouchGround", .1f);

            // since when we touch the ground, make sure to retreat to the default original rotation as well
            // Getting reference to the default rotation
            if (InputController.Instance.M_PreviousMovement == InputController.PreviousMovement.Right) transform.right = Vector2.right;
            else if (InputController.Instance.M_PreviousMovement == InputController.PreviousMovement.Left) transform.right = Vector2.left;
        }
        public void OnStay_FlyingSpikeGround(Collider2D collider)
        {
            m_canJump = true;
            this.transform.parent = collider.transform;

            // when we're at the staying stage we need to make sure the player doesn't get slipped
        }
        public void OnExit_FlyingSpikeGround(Collider2D collider)
        {
            this.transform.parent = null;

            m_canJump = false;
            m_anim.SetBool("IsJumping", true);

            Debug.Log($"OnExit called");

        }
        #endregion




        #endregion

        #region SlowMotionSettings

        [ContextMenu("Apply Slow Motion")]
        public void DoSlowMotion()
        {
            GameplayController.Instance.ApplySlowMotionEffect();
        }
        [ContextMenu("Remove Slow Motion")]
        public void UnDoSlowMotion()
        {
            GameplayController.Instance.RemoveSlowMotionEffect();
        }

        #endregion


        private void OnDisable()
        {
            Debug.Log("Player Disabled");
            Invoke(nameof(RestartScene), 3f);
            Invoke(nameof(DoFade), 1.5f);
        }

        void DoFade()
        {
            GameplayController.Instance.M_CanvasManager.FadeImage.GetComponent<Animator>().CrossFade("FadeIn", .1f);
        }
        void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

}
