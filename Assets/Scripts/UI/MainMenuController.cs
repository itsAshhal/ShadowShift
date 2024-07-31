using ETFXPEL;
using ShadowShift.DataModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShadowShift.UI
{
    public class MainMenuController : Singleton<MainMenuController>
    {
        [SerializeField] Image m_fadeImage;
        [SerializeField] float m_fadingStartDuration;
        [SerializeField] ControlType m_controlType;
        [SerializeField] MainMenuCanvas m_mainMenuCanvas;
        [SerializeField] Vector2 m_selectedScaling;
        [SerializeField] Vector2 m_deselectedScaling;
        [SerializeField] float m_scalingDuration;
        public UnityEvent m_startActions;
        void Start()
        {
            StartCoroutine(FadeCoroutine());
            m_startActions?.Invoke();

            if (GameData.LoadData().Controls == "Buttons") ChangeControls("Buttons");
            else ChangeControls("Swipe");
        }

        IEnumerator FadeCoroutine()
        {
            yield return new WaitForSeconds(m_fadingStartDuration);
            m_fadeImage.GetComponent<Animator>().CrossFade("FadeOut", .1f);
        }



        public void ChangeControls(string controlString)
        {
            // load the controls from the local file first
            var playerData = GameData.LoadData();
            m_controlType.M_Controls = controlString == "Buttons" ? ControlType.Controls.Buttons : ControlType.Controls.Swipe;

            // now save to local file as well
            GameData.SaveData(new PlayerData {
                Controls = m_controlType.M_Controls.ToString(),
                Stage = playerData.Stage ,
                CameraOrthoSize = playerData.CameraOrthoSize,
                CameraHeight = playerData.CameraHeight
            });

            Debug.Log($"NewData is saved to {m_controlType.M_Controls.ToString()}");


            if (m_controlType.M_Controls == ControlType.Controls.Buttons)
            {
                m_mainMenuCanvas.AnimateImageScale(m_mainMenuCanvas.ButtonsControlImage, m_selectedScaling, m_scalingDuration);
                m_mainMenuCanvas.AnimateImageScale(m_mainMenuCanvas.SwipeControlImage, m_deselectedScaling, m_scalingDuration);
            }
            else
            {
                m_mainMenuCanvas.AnimateImageScale(m_mainMenuCanvas.SwipeControlImage, m_selectedScaling, m_scalingDuration);
                m_mainMenuCanvas.AnimateImageScale(m_mainMenuCanvas.ButtonsControlImage, m_deselectedScaling, m_scalingDuration);
            }
        }
    }

}