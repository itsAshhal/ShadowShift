using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace ShadowShift.Fusion
{
    [ExecuteAlways]
    public class Lobby_UI : MonoBehaviour
    {
        public static Lobby_UI Instance;
        public float AnimationDuration = .25f;
        public Animator TopAnim;
        public Animator RightAnim;
        public Animator BottomAnim;
        [Tooltip("The actual gameplay UI which will be used by the players to either resume or leave the room as per wish")]
        public GameObject GameplayUIContainer;
        public Button LeaveRoomBtn;
        public GameObject LeaveRoomParent;
        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
        }

        private void Start()
        {
            EnableGameplayUI(false);
            LeaveRoomParent.SetActive(false);
        }


        /// <summary>
        /// Call this method from the Host when he starts the gameplay after completing and verifying all the votes
        /// </summary>
        /// 
        [ContextMenu("Remove Lobby UI")]
        public void RemoveLobbyUI()
        {
            StartCoroutine(RemoveLobbyUI_Coroutine());
        }
        IEnumerator RemoveLobbyUI_Coroutine()
        {
            TopAnim.CrossFade("Disappear", .1f);
            yield return new WaitForSeconds(AnimationDuration);
            RightAnim.CrossFade("Disappear", .1f);
            yield return new WaitForSeconds(AnimationDuration);
            BottomAnim.CrossFade("Disappear", .1f);
        }

        /// <summary>
        /// Enables the Gameplay UI which is actually needed in the gameplay so the player can either resume or leave the room on his own wish
        /// </summary>
        /// <param name="isActive">TRUE => sets the UI enabled otherwise disabled</param>
        public void EnableGameplayUI(bool isActive)
        {
            GameplayUIContainer.SetActive(isActive);
        }


    }
}
