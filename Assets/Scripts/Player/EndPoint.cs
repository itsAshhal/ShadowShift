using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using ShadowShift.DataModels;
using UnityEngine.SceneManagement;

namespace ShadowShift.Player
{
    /// <summary>
    /// This endpoint will be used at the end of each stage to let the user know that the stage has been completed
    /// </summary>
    /// 
    [RequireComponent(typeof(BoxCollider2D))]
    public class EndPoint : MonoBehaviour
    {
        [Tooltip("So when the player reaches the end point, the main camera transitions to this for smooth animation effect")]
        [SerializeField] CinemachineVirtualCamera m_playerVcam;
        [Tooltip("This endPoint has to know the mainPlayer tag so we can trigger certain things")]
        [SerializeField] string m_playerTag;
        [SerializeField] float m_startFadeinAfter = 2f;

        private void Awake()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        public void OnReachEndPoint(PlayerController playerController)
        {
            // set this vcam's priority
            m_playerVcam.Priority = CinematicsController.Instance.MainCamera.Priority + 1;

            // turn the gameplay state off as well so the user can't do anything with the player
            GameplayController.Instance.M_GameplayState = GameplayState.Off;

            // turn off the rigidbody of the player as well
            playerController.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(m_playerTag) == false) return;
            OnReachEndPoint(collision.GetComponent<PlayerController>());

            Invoke(nameof(DoFade), m_startFadeinAfter);

            // save the current stage
            PlayerData loadedData = GameData.LoadData();

            // since there is no data saved in the first place
            if (loadedData.Stage == 0)
            {
                // no stage data is found
                GameData.SaveData(new PlayerData
                {
                    Controls = loadedData.Controls,
                    Stage = 1,
                });
            }
            else
            {
                GameData.SaveData(new PlayerData
                {
                    Controls = loadedData.Controls,
                    Stage = loadedData.Stage + 1,
                });
            }

            Debug.Log($"CurerntStage saved is {GameData.LoadData().Stage - 1}, NextStage is {GameData.LoadData().Stage}");

        }

        void DoFade()
        {
            GameplayController.Instance.M_CanvasManager.FadeImage.GetComponent<Animator>().CrossFade("FadeIn", .1f);
            Invoke(nameof(RestartScene), 2.5f);
        }

        void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}
