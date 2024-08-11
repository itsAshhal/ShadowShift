using ShadowShift.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Fusion;
using System;
using UnityEngine.UI;
using TMPro;

namespace ShadowShift.Fusion
{
    public class PlayerControllerFusion : NetworkBehaviour
    {
        [SerializeField] float m_moveSpeed;
        [SerializeField] float m_jumpSpeed;
        [SerializeField] bool m_playerCanRotateWhenNotTouchingTheGound = true;
        [SerializeField] float m_rotationSpeed = 2f;
        public bool m_canJump = true;
        public Quaternion CustomRotation;


        [Networked]
        public Color SpriteColor { get; set; }
        [Networked]
        public int TotalVotes { get; set; }

        private bool m_canVote = true;

        private Rigidbody2D m_rb;
        [SerializeField] Animator m_anim;  // this is the main object, the black player as the child of this controller
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




            m_originalRotation = transform.rotation;

            if (LobbyManager.Instance == null) return;

            // now we need to bind the specific function to the colorPaletter action
            LobbyManager.Instance.OnColorPaletterChange += OnPlayerSpriteColorChange;

            // we need to bind actions to detect the vote changes
            LobbyManager.Instance.OnChangeVote += OnVoteChanged;

        }

        public override void FixedUpdateNetwork()
        {
            Debug.Log($"TouchCount {Input.touchCount}");
            //transform.rotation = CustomRotation;

            Debug.Log($"My input authority is {HasInputAuthority}");

            try
            {
                if (GetInput(out LobbyNetworkInputData data))
                {
                    // normalize the direction so we can multiply it for speed
                    data.direction.Normalize();

                    m_rb.velocity = m_moveSpeed * Runner.DeltaTime * data.direction;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"FixedUdpateNetwork issue => {e.Message}");
            }
        }



        void MovePlayerRight()
        {

            Debug.Log($"Actions working");
            Vector2 movementVector = Vector2.zero;
            movementVector = Vector2.right * m_moveSpeed;
            m_rb.velocity = new Vector2(movementVector.x, m_rb.velocity.y);

            // Rotate player to face right using Quaternion
            transform.rotation = Quaternion.Euler(0, 0, 0); // No rotation on the Z axis for 2D
            Debug.Log($"Rotating Right {transform.rotation.eulerAngles}");
        }

        void MovePlayerLeft()
        {

            Debug.Log("Actions working");
            Vector2 movementVector = Vector2.left * m_moveSpeed;
            m_rb.velocity = new Vector2(movementVector.x, m_rb.velocity.y);

            // Rotate player to face left using Quaternion
            transform.rotation = Quaternion.Euler(0, 180f, 0); // Rotate 180 degrees around the Y axis
            Debug.Log($"Rotating Left {transform.rotation.eulerAngles}");
        }

        public void OnVoteChanged()
        {
            // we need to check if we can still vote or not, as we can't just keep on voting infinitely
            // One player can either increase the vote by 1 or decrease it by 1 if he's already voted
            RPC_ChangeVoteOnServer(m_canVote);  // we only need to increase the vote by 1 for only 1 client

            if (HasStateAuthority) TotalVotes = m_canVote == true ? TotalVotes += 1 : TotalVotes -= 1;

            // Change the current state of the vote as well
            m_canVote = !m_canVote;



        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPC_ChangeVoteOnServer(bool canVote, RpcInfo info = default)
        {
            RPC_ChangeVoteOnClient(canVote, info.Source);
        }
        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        public void RPC_ChangeVoteOnClient(bool canVote, PlayerRef messageSource)
        {
            // Get the previous votes
            int previousVotes = canVote == true ? int.Parse(LobbyManager.Instance.TotalVotesText.text) + 1 : int.Parse(LobbyManager.Instance.TotalVotesText.text) - 1;

            // now set the votes properly
            LobbyManager.Instance.TotalVotesText.text = previousVotes.ToString();

            // change the networkedState as well
            TotalVotes = previousVotes;

            Debug.Log($"TotalVotes are {TotalVotes}");
        }


        public void OnPlayerSpriteColorChange(Color color)
        {
            if (HasInputAuthority == false) return;
            Debug.Log($"Color changed from PlayerControllerFusion, color -> {color}");

            SpriteColor = color;

            m_anim.GetComponent<SpriteRenderer>().color = SpriteColor;

            RPC_ChangeSpriteColorOnServer(color);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPC_ChangeSpriteColorOnServer(Color message, RpcInfo info = default)
        {
            RPC_ChangeSpriteColorOnClients(message, info.Source);
        }
        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        public void RPC_ChangeSpriteColorOnClients(Color color, PlayerRef messageSource)
        {

            m_anim.GetComponent<SpriteRenderer>().color = color;
            SpriteColor = color;
        }

        #endregion

        #region Control Actions

        private void OnPlayerJump()
        {
            Debug.Log($"Actions working");
            if (m_canJump == false) return;
            m_rb.velocity = new Vector2(m_rb.velocity.x, m_jumpSpeed);
        }

        private void OnMovePlayerNoWhere()
        {
            Debug.Log($"Actions working");
            m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
        }

        private void OnMovePlayerLeft()
        {
            MovePlayerLeft();
        }

        private void OnMovePlayerRight()
        {
            MovePlayerRight();
        }

        #endregion

        #region Trigger Actions

        #region Ground

        [Header("Ground settings")]
        [SerializeField] LayerMask m_groundLayermask;

        public void OnEnterGround(Collider2D collider)
        {

        }
        public void OnStayGround(Collider2D collider)
        {
        }
        public void OnExitGround(Collider2D collider)
        {

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
        }
        public void OnStay_Hide(Collider2D collider)
        {

        }
        public void OnExit_Hide(Collider2D collider)
        {
            m_anim.SetBool("IsHiding", false);
            Fade(m_playerEyeSprite, true, m_imageFadeDuration);
            M_PlayerHiddenState = PlayerHiddenState.Open;
        }

        #endregion


        #endregion





    }


}