using ETFXPEL;
using ShadowShift.DataModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShadowShift.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public static MainMenuController Instance;

        [SerializeField] Image m_fadeImage;
        [SerializeField] float m_fadingStartDuration;
        [SerializeField] ControlType m_controlType;
        [SerializeField] MainMenuCanvas m_mainMenuCanvas;
        [SerializeField] Vector2 m_selectedScaling;
        [SerializeField] Vector2 m_deselectedScaling;
        [SerializeField] float m_scalingDuration;
        public UnityEvent m_startActions;
        public AudioSource MusicAudio;
        public Transform SavedColorsContentParent;
        public GameObject SavedColorPrefab;  // so don't have to change a lot of things
        public List<GameObject> Loaded_UI_Colors;
        public float DisplayColorsAppearanceDuration = .5f;
        public Image MainBackground;

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
        }

        void Start()
        {
            StartCoroutine(FadeCoroutine());
            m_startActions?.Invoke();

            if (GameData.LoadData().Controls == "Buttons") ChangeControls("Buttons");
            else ChangeControls("Swipe");


            // Loading music data
            LoadMusicValue();

            SetSavedColors();

            DisplayToggleColorScreen();

        }

        /// <summary>
        /// Whether or not we should display the toggle screen to the user!?, based
        /// on his stages completed
        /// </summary>
        public void DisplayToggleColorScreen()
        {
            if (GameData.LoadColorData() == null)
            {
                m_mainMenuCanvas.ToggleControllerScreen.SetActive(false);
                return;
            }

            bool displayCondition = (
                GameData.LoadData().Stage >= m_mainMenuCanvas.TotalStagesToCompleteToUnlockColorTheory
                &&
                GameData.LoadColorData().Count > 0
            );

            // also we need to check one more thing, if there are any colors then show this option, otherwise there's no point

            m_mainMenuCanvas.ToggleControllerScreen.SetActive(displayCondition);


        }

        IEnumerator FadeCoroutine()
        {
            yield return new WaitForSeconds(m_fadingStartDuration);
            m_fadeImage.GetComponent<Animator>().CrossFade("FadeOut", .1f);
        }

        public void SetSavedColors()
        {
            // so we can put all the colors into the UI and the us can then select any of those for his pleasure
            foreach (var data in Loaded_UI_Colors) Destroy(data.gameObject);
            Loaded_UI_Colors.Clear();
            StartCoroutine(DisplaySavedColors());
        }

        IEnumerator DisplaySavedColors()
        {
            if (GameData.LoadColorData() != null)
            {
                foreach (var data in GameData.LoadColorData())
                {
                    var newLoadedColor = Instantiate(SavedColorPrefab, SavedColorsContentParent);
                string hexColor = data.HexColor;

                    Color newColor;
                    if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
                    {
                        // Apply the new color to the button
                        newLoadedColor.transform.GetChild(0).GetComponent<Image>().color = newColor;
                    }

                    Loaded_UI_Colors.Add(newLoadedColor);
                    yield return new WaitForSeconds(DisplayColorsAppearanceDuration);
                }
            }


        }

        public void LoadMusicValue()
        {
            var musicValue = GameData.LoadData().MusicValue;
            Debug.Log($"music value obtained is {musicValue}");
            m_mainMenuCanvas.MusicSlider.value = musicValue;
            MusicAudio.volume = musicValue;
        }

        public void ChangeMainMenuBackground(Color newBackgroundColor) => MainBackground.color = newBackgroundColor;


        public void ChangeControls(string controlString)
        {
            // load the controls from the local file first
            var playerData = GameData.LoadData();
            m_controlType.M_Controls = controlString == "Buttons" ? ControlType.Controls.Buttons : ControlType.Controls.Swipe;

            // now save to local file as well
            GameData.SaveData(new PlayerData
            {
                Controls = m_controlType.M_Controls.ToString(),
                Stage = playerData.Stage,
                CameraOrthoSize = playerData.CameraOrthoSize,
                CameraHeight = playerData.CameraHeight,
                MusicValue = playerData.MusicValue
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