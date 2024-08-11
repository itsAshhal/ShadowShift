using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShadowShift.InputController;

namespace ShadowShift.Fusion
{
    /// <summary>
    /// This controller class mainly handles the movement of the player when they are in the lobby.
    /// So it gives the players some freedom to move around while deciding to start the game or until all the players join
    /// and are ready to participate in the game. 
    /// "NOTE" => this class only allows the players to move around with swipe mechanics as there is to be other
    /// UI elements so we are not using the arrow buttons for movements
    /// </summary>
    public class LobbyInputController : MonoBehaviour
    {
        public enum DirectionState
        {
            Left, Right, None
        }
        public DirectionState M_DirectionState;

        public static LobbyInputController Instance;
        [SerializeField] float m_touchMovementThreshold = .1f;

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
        }

        private void Start()
        {
            M_DirectionState = DirectionState.None;
        }


        private void Update()
        {
            ManageSwipeControls();
        }
        void ManageSwipeControls()
        {
            if (Input.touchCount > 0)
            {
                // make sure there is atleast a single touch on the screen
                Touch touch = Input.GetTouch(0); // we only need one touch

                float value = Screen.width / 2;
                if (touch.position.x > value) return;

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
                        // we're moving right
                        Debug.Log($"Lobby movement, moving right");
                        M_DirectionState = DirectionState.Right;
                    }
                    else if (deltaPosition.x < (0f - m_touchMovementThreshold))
                    {
                        // we're moving left
                        Debug.Log($"Lobby movement, moving left");
                        M_DirectionState = DirectionState.Left;
                    }
                    //else M_PlayerMovement = PlayerMovement.None;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    // we have stopped moving
                    M_DirectionState = DirectionState.None;
                }
            }

        }


    }
}
