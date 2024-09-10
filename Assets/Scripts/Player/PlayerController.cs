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
