using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadowShift.UI;
using ShadowShift.DataModels;

namespace ShadowShift.Player
{
    public class GameplayController : Singleton<GameplayController>
    {
        [SerializeField] ControlType m_controlType;
        public ControlType M_ControlType => this.m_controlType;
        [SerializeField] CanvasManager m_canvasManager;
        [Tooltip("Used in scaling the control buttons when we change to a new control type")]
        [SerializeField] Vector2 m_controlScaling;
        [SerializeField] float m_scalingDuration;




        void Start()
        {
            LoadControls();
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