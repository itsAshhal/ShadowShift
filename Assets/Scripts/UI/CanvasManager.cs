using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Events;
using Cinemachine;
using ShadowShift.DataModels;


namespace ShadowShift.UI
{
    public enum SliderType
    {
        Height, Zoom
    }



    /// <summary>
    /// Manages the whole canvas elements (UI)
    /// </summary>
    public class CanvasManager : MonoBehaviour
    {
        public UnityEvent OnGamePause;
        public UnityEvent OnGameResume;
        public Button LeftMoveButton;
        public Button RightMoveButton;
        public Button JumpButton;
        [Tooltip("This is the parent for each button, so we can turn this off and on and not each button separately")]
        public GameObject MovementButtonsParent;
        public Image FadeImage;
        public Slider ZoomSlider;
        public Slider HeightSlider;


        public void OnValueChange_CameraUpDown(float value)
        {
            var camera = CinematicsController.Instance.MainCamera;

            var transposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer == null) return;

            transposer.m_TrackedObjectOffset = new Vector3(transposer.m_TrackedObjectOffset.x, value * 6f, transposer.m_TrackedObjectOffset.z);

            var loadedData = GameData.LoadData();

            GameData.SaveData(new PlayerData
            {
                Stage = loadedData.Stage,
                Controls = loadedData.Controls,
                CameraOrthoSize = loadedData.CameraOrthoSize,
                CameraHeight = transposer.m_TrackedObjectOffset.y,
                MusicValue = loadedData.MusicValue
            });

        }

        public void OnValueChange_CameraZoom(float value)
        {
            // value == 0, zoom = 6
            // value == 14, zoom = 14
            var camera = CinematicsController.Instance.MainCamera;

            value = Mathf.Clamp(value, 0f, 1f);

            float minSize = 6f;
            float maxSize = 14f;

            camera.m_Lens.OrthographicSize = Mathf.Lerp(minSize, maxSize, value);


            var loadedData = GameData.LoadData();

            GameData.SaveData(new PlayerData
            {
                Stage = loadedData.Stage,
                Controls = loadedData.Controls,
                CameraOrthoSize = camera.m_Lens.OrthographicSize,
                CameraHeight = loadedData.CameraHeight,
                MusicValue = loadedData.MusicValue
            });
        }


        public float GetSliderValueFromZoom(float zoom)
        {
            float minSize = 6f;
            float maxSize = 14f;
            return (zoom - minSize) / (maxSize - minSize);
        }


        // Changing slider values manually
        /// <summary>
        /// Change the slider settings automatically, especially when
        /// you're loading the data
        /// </summary>
        /// <param name="sliderType">Give type slider type</param>
        /// <param name="value">Current value of the slider</param>
        public void ChangeSliderValue(SliderType sliderType, float value)
        {
            switch (sliderType)
            {
                case SliderType.Height: this.HeightSlider.value = value; break;
                case SliderType.Zoom: this.ZoomSlider.value = value; break;
            }

        }


        public void OnPress_Pause()
        {
            OnGamePause?.Invoke();
        }
        public void OnPress_Continue()
        {
            OnGameResume?.Invoke();
        }


        public void SetTimeScale(float value)
        {
            Debug.Log($"TimeScale value changed to {value}");
            Time.timeScale = value;
        }


        /// <summary>
        /// Used to set the UI active and inactive
        /// </summary>
        /// <param name="uiState">TRUE => will be on, FALSE => will be off</param>
        public void SetUI_Buttons(bool uiState = false)
        {
            if (!uiState)
            {
                LeftMoveButton.gameObject.SetActive(false);
                RightMoveButton.gameObject.SetActive(false);
                JumpButton.gameObject.SetActive(false);
            }
            else
            {
                LeftMoveButton.gameObject.SetActive(true);
                RightMoveButton.gameObject.SetActive(true);
                JumpButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Used to set the UI active and inactive
        /// </summary>
        /// <param name="uiState">TRUE => will be on, FALSE => will be off</param>
        public void Set_UI_Swipe(bool uiState = false)
        {

        }




    }

}