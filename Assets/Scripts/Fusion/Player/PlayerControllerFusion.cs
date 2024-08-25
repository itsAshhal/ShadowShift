using UnityEngine;
using DG.Tweening;
using Fusion;
using System;
using Cinemachine;
using TMPro;
using System.Collections;

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
        [Tooltip("Unparent this vcam on start after checking the input authority")]
        [SerializeField] CinemachineVirtualCamera m_playerVcam;
        [SerializeField] TMP_Text m_nicknameText;


        [Networked]
        public Color SpriteColor { get; set; }
        [Networked]
        public int TotalVotes { get; set; }
        [Networked]
        public string nickName { get; set; }

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


            // unparenting the cinemachine vcam
            m_playerVcam.transform.SetParent(null);

            // ok so lets try a trick, if the player doesn't have the input authority, then destroy the camera
            if (!HasInputAuthority) Destroy(m_playerVcam.gameObject);

            //AnimatePlayerNickname();

            // bind this function to OnPlayerJoined callback of the FusionConnection
            if (HasInputAuthority)
            {
                if (FusionConnection.Instance != null)
                {
                    FusionConnection.Instance._OnPlayerJoined += this.AnimatePlayerViaFusionSync;
                }

            }


            m_originalRotation = transform.rotation;

            if (LobbyManager.Instance == null) return;

            // now we need to bind the specific function to the colorPaletter action
            LobbyManager.Instance.OnColorPaletterChange += OnPlayerSpriteColorChange;

            // we need to bind actions to detect the vote changes
            LobbyManager.Instance.OnChangeVote += OnVoteChanged;

            // check if we're the host so we can start the gameplay with the lobby canvas UI animations
            if (Lobby_UI.Instance == null) return;

            Debug.Log($"OnClickStartGameByHost is binded");
            LobbyManager.Instance.OnClickStartGameByHost += this.OnGameStartByHost;

        }


        /// <summary>
        /// This function will be called when someone joins but here on each client which makes it easier to update things when new clients come up
        /// </summary>
        /// <param name="runner">The runner which has joined</param>
        /// <param name="player">player ref of the runner that has joined</param>
        public void AnimatePlayerViaFusionSync(NetworkRunner runner, PlayerRef player)
        {
            // whenever a new player joins, this callback will be used and called so it will update the nicknames on the player
            Debug.Log($"PlayerNickname Syncing is called");

            // lets try changing the nicknames via the networked proerties this time and see what happens
            //AnimatePlayerNickname();

            // lets try a different method, use InvokeRepeating for atleast next 5 seconds to keep others updated
            StartCoroutine(AnimatePlayerNicknameCoroutine());

            //AnimatePlayerNickname();
        }

        IEnumerator AnimatePlayerNicknameCoroutine()
        {
            for (int i = 1; i <= 5; i++)
            {
                yield return new WaitForSeconds(1f);
                AnimatePlayerNickname();
            }
        }

        public void AnimatePlayerNickname()
        {
            // lets first test if its working on local devices or not
            if (HasInputAuthority == false) return;
            RPC_ChangeNicknameOnServer(FusionConnection.Instance.Nickname);

        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPC_ChangeNicknameOnServer(string nickname, RpcInfo info = default)
        {
            RPC_ChangeNicknameOnClients(nickname);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        public void RPC_ChangeNicknameOnClients(string nickname)
        {
            var anim = m_nicknameText.GetComponent<Animator>();
            m_nicknameText.text = nickname;
            anim.CrossFade("Appear", .1f);
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


        public void OnGameStartByHost()
        {
            // now call RPCs so we can animate the UI on all the clients and start the actual gameplay
            Debug.Log($"OnClickStartGameByHost RPC server called");
            if (Runner.IsServer == false) return;
            RPC_StartGameOnServer();
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPC_StartGameOnServer(RpcInfo info = default)
        {
            // call the RPC for clients
            RPC_StartGameOnClients();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        public void RPC_StartGameOnClients()
        {
            Debug.Log($"RPC start game called on all clients");
            // use the Lobby UI to animate the UI
            Lobby_UI.Instance.RemoveLobbyUI();

            // since this RPC is working on all the clients, make a smooth transition from the camera as well
            LobbyManager.Instance.LobbyCamera.Priority = 5; // make sure to set it to 11 in the editor
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