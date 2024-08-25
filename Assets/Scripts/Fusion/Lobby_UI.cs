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
        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
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


    }
}
