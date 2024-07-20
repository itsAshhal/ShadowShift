using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        private Quaternion m_originalRotation;

        #region Unity
        void Awake()
        {
            //M_PlayerMovement = PlayerMovement.None;
            m_rb = GetComponent<Rigidbody2D>();
            //m_anim = GetComponent<Animator>();

            // setting up input controls with actions
            InputController.Instance.OnMoveRight += OnMovePlayerRight;
            InputController.Instance.OnMoveLeft += OnMovePlayerLeft;
            InputController.Instance.OnNothing += OnMovePlayerNoWhere;
            InputController.Instance.OnPressJump += OnPlayerJump;

            m_originalRotation = transform.rotation;
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
            //m_canJump = true;

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
        }
        public void OnStay_Hide(Collider2D collider)
        {

        }
        public void OnExit_Hide(Collider2D collider)
        {
            m_anim.SetBool("IsHiding", false);
            Fade(m_playerEyeSprite, true, m_imageFadeDuration);
        }

        #endregion

        #endregion



    }
}
