using System.Collections;
using System.Collections.Generic;
using ShadowShift;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowShift
{
    public class ControlTypeManager : MonoBehaviour
    {
        [SerializeField] ControlType m_controlType;

        public void OnPress_Swipe()
        {
            m_controlType.M_Controls = ControlType.Controls.Swipe;
            PressAction();
        }
        public void OnPress_Buttons()
        {
            m_controlType.M_Controls = ControlType.Controls.Buttons;
            PressAction();
        }

        private void PressAction()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

}