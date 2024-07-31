using System;
using System.Collections;
using System.Collections.Generic;
using ShadowShift.Player;
using ShadowShift.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ShadowShift
{
    /// <summary>
    /// Main class for handling input controls, both
    /// for SWIPE and BUTTONS
    /// </summary>
    public class InputController : MonoBehaviour
    {

        public Action<Touch> OnPressJump;
        public Action<Touch> OnMoveRight;
        public Action<Touch> OnMoveLeft;
        public Action<Touch> OnNothing;
        public string ControlsTag = "Controls";
        public enum PlayerMovement
        {
            Right, Left, None
        }
        public enum PreviousMovement
        {
            Right, Left
        }
        public PreviousMovement M_PreviousMovement;
        public PlayerMovement M_PlayerMovement;
        [Tooltip("If the finger after touched moves farther than this point either left or right, then the movement is registered")]
        [SerializeField] float m_touchMovementThreshold = .1f;


        public static InputController Instance;
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
            M_PlayerMovement = PlayerMovement.None;

            // ok so there are 2 things worth noticing here
            // if the control type is Swipe, we need to make sure in the update function and 
            // if the control type is of Buttons, we need to make sure in Start function



        }


        void ManageSwipeControls()
        {
            if (Input.touchCount > 0)
            {
                // make sure there is atleast a single touch on the screen
                Touch touch = Input.GetTouch(0); // we only need one touch

                float value = Screen.width / 2;
                if (touch.position.x > value)
                {
                    M_PlayerMovement = PlayerMovement.None;
                    return;
                }

                // check touchPhases
                if (touch.phase == TouchPhase.Began)
                {
                    // m_touchStartPosition = touch.position;
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 deltaPosition = touch.deltaPosition;
                    Debug.Log($"DeltaPosition {deltaPosition}");

                    if (deltaPosition.x > (0f + m_touchMovementThreshold))
                    {
                        M_PlayerMovement = PlayerMovement.Right;
                    }
                    else if (deltaPosition.x < (0f - m_touchMovementThreshold))
                    {
                        M_PlayerMovement = PlayerMovement.Left;
                    }
                    //else M_PlayerMovement = PlayerMovement.None;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    if (M_PlayerMovement == PlayerMovement.Right) M_PreviousMovement = PreviousMovement.Right;
                    else if (M_PlayerMovement == PlayerMovement.Left) M_PreviousMovement = PreviousMovement.Left;

                    Debug.Log($"Getting previous movement {M_PlayerMovement.ToString()}");
                    M_PlayerMovement = PlayerMovement.None;
                }
            }

        }

        void ManageJumpControls()
        {
            int touchCount = Input.touchCount;

            // we need to remain ahead at the last touch
            if (touchCount == 0) return;
            Touch touch = Input.GetTouch(touchCount - 1);

            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < (Screen.width / 2)) return;

                OnPressJump?.Invoke(touch);
            }
        }

        // This method checks if a touch has hit any UI element
        public bool IsTouchOverUIWithTag(string tag)
        {
            // Check if there are any touches
            if (Input.touchCount > 0)
            {
                // Get the first touch
                Touch touch = Input.GetTouch(0);

                // Check if the touch is over a UI element
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    // Create a pointer event data object
                    PointerEventData pointerData = new PointerEventData(EventSystem.current);
                    pointerData.position = touch.position;

                    // Raycast to find the UI element
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerData, raycastResults);

                    // Check if any of the raycast results have the specified tag
                    foreach (RaycastResult result in raycastResults)
                    {
                        if (result.gameObject.CompareTag(tag))
                        {
                            // Return true if a UI element with the specified tag is hit
                            return true;
                        }
                    }
                }
            }

            // Return false if there are no touches or no UI element with the specified tag is hit
            return false;
        }


        void Update()
        {
            // first make sure that are we touching a UI element or just the simple screen space
            //if (!IsTouchOverUIWithTag(ControlsTag)) return;
            if (GameplayController.Instance.M_GameplayState == GameplayState.Off) return;

            if (GameplayController.Instance == null) return;
            if (GameplayController.Instance.M_ControlType.M_Controls == ControlType.Controls.Swipe)
            {
                ManageSwipeControls();
                ManageJumpControls();
            }


            // This part of the code is needed for dynamic movement assignment
            if (M_PlayerMovement == PlayerMovement.Right) OnMoveRight?.Invoke(new Touch());
            else if (M_PlayerMovement == PlayerMovement.Left) OnMoveLeft?.Invoke(new Touch());
            else OnNothing?.Invoke(new Touch());

        }


        public void OnButtonHold_Right()
        {
            M_PlayerMovement = PlayerMovement.Right;
            Debug.Log($"ButtonHold Right");
        }
        public void OnButtonHold_Left()
        {
            M_PlayerMovement = PlayerMovement.Left;
            Debug.Log($"ButtonHold Left");
        }
        public void OnButtonUp_RightLeft()
        {
            M_PlayerMovement = PlayerMovement.None;
        }
        public void OnButtonDown_Jump()
        {
            //M_PlayerMovement = PlayerMovement.Right;
            OnPressJump?.Invoke(new Touch());
        }


    }
}