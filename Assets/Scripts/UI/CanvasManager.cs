using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace ShadowShift.UI
{
    /// <summary>
    /// Manages the whole canvas elements (UI)
    /// </summary>
    public class CanvasManager : MonoBehaviour
    {
        public Button LeftMoveButton;
        public Button RightMoveButton;
        public Button JumpButton;
        [Tooltip("This is the parent for each button, so we can turn this off and on and not each button separately")]
        public GameObject MovementButtonsParent;




        void Start()
        {
            SetOriginalScale();
        }

        void SetOriginalScale()
        {

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