using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadowShift.UI;
using ShadowShift.DataModels;
using System;
using Random = UnityEngine.Random;
using Cinemachine;

namespace ShadowShift.Player
{
    public enum GameplayState
    {
        On, Off
    }

    public class GameplayController : MonoBehaviour
    {
        [SerializeField] ControlType m_controlType;
        public ControlType M_ControlType { get { return m_controlType; } }
        [SerializeField] CanvasManager m_canvasManager;
        public CanvasManager M_CanvasManager { get { return this.m_canvasManager; } }
        [Tooltip("Used in scaling the control buttons when we change to a new control type")]
        [SerializeField] Vector2 m_controlScaling;
        [SerializeField] float m_scalingDuration;
        [SerializeField] Stage[] m_allStages;

        public GameplayState M_GameplayState;
        public PlayerController MainPlayer { get; private set; }


        public static GameplayController Instance;
        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Destroy(this);
            }
            else Instance = this;
        }


        void Start()
        {
            M_GameplayState = GameplayState.On;
            LoadControls();

            // start the fading effect as well
            m_canvasManager.FadeImage.GetComponent<Animator>().CrossFade("FadeOut", .1f);


            MainPlayer = FindObjectOfType<PlayerController>();

            LoadNewStage(GameData.LoadData().Stage);
            LoadCameraSettings();
        }

        void LoadCameraSettings()
        {
            var playerData = GameData.LoadData();
            Debug.Log($"CameraSettings Loaded, orthoSize {playerData.CameraOrthoSize}, height {playerData.CameraHeight}");
            /*m_canvasManager.OnValueChange_CameraUpDown(playerData.CameraHeight);
            m_canvasManager.OnValueChange_CameraZoom(playerData.CameraOrthoSize);*/

            var cinemachineCamera = CinematicsController.Instance.MainCamera;
            var framingTransposer = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            framingTransposer.m_TrackedObjectOffset = new Vector3(
                 framingTransposer.m_TrackedObjectOffset.x,
                 playerData.CameraHeight,
                  framingTransposer.m_TrackedObjectOffset.z
                );

            cinemachineCamera.m_Lens.OrthographicSize = playerData.CameraOrthoSize;

            // set the sliders as well
            m_canvasManager.ChangeSliderValue(SliderType.Height, playerData.CameraHeight / 6f);
            m_canvasManager.ChangeSliderValue(SliderType.Zoom, m_canvasManager.GetSliderValueFromZoom(playerData.CameraOrthoSize));
        }

        void LoadNewStage(int stageIndex)
        {
            try
            {
                Debug.Log($"StageIndex is {stageIndex}");
                Stage spawnedStage = Instantiate(m_allStages[stageIndex], Vector3.zero, Quaternion.identity);
                MainPlayer.transform.position = spawnedStage.PlayerSpawnPosition.position;
            }
            catch (Exception e)
            {
                Debug.Log($"Stage error {e.Message}");
            }
        }

        void LoadControls()
        {
            // we need to set the gameplay UI at start for the game

            var playerData = GameData.LoadData();  // we don't need to worry about the file existence since this is the place
            // where we must have the controls
            m_controlType.M_Controls = playerData.Controls == "Buttons" ? ControlType.Controls.Buttons : ControlType.Controls.Swipe;

            m_canvasManager.SetUI_Buttons(m_controlType.M_Controls == ControlType.Controls.Buttons);



        }

        public Vector2 GetRandomDirection2D()
        {
            float angle = Random.Range(0f, 360f);
            float radian = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }


    }

}