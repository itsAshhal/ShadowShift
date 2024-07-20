using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShadowShift.UI
{
    /// <summary>
    /// Responsible for controling the bottom UI part with its animations as well
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] Button m_mainButton;
        [SerializeField] Sprite m_defaultSprite;
        [SerializeField] Sprite m_settingSprite;
        [SerializeField] Animator[] m_defaultButtonsAnim;  // has different set of icons
        [SerializeField] Animator[] m_settingsButtonAnim;  // has different set of icons
        [Tooltip("The time taken between each button completes its animation, then next button comes after this time perios")]
        [SerializeField] float m_durationBetweenEachButtonAnim;
        [Tooltip("As there are only 2 settings categories, duration between those two is this var")]
        [SerializeField] float m_durationBetweenMajorSwitching;
        [Tooltip("Since each button has a logo inside it as well which needs to be animated to, that's the time perios required for it")]
        [SerializeField] float m_durationBetweenLogoAndItsParent = .0f;
        [SerializeField] float m_mainButtonIconDuration = .1f;

        public enum SettingState
        {
            Default, Settings
        }
        public SettingState M_SettingState;

        void Start()
        {
            M_SettingState = SettingState.Default;
            Init();
        }

        public void OnClickSettings()
        {
            // remember, the button will remain the same on its location
            // only its image and icon will be changed
            StartCoroutine(SwitchingCoroutine());

        }

        void Init()
        {
            // making sure, settings button are not displayed when the screen shows up
            foreach (var anim in m_settingsButtonAnim) anim.CrossFade("Disappear", .1f);

            // assign the Listener as well
            m_mainButton.onClick.AddListener(OnClickSettings);
        }

        void SetButtonsInteraction(Animator[] buttonAnim, bool interaction)
        {
            foreach (var anim in m_defaultButtonsAnim)
            {
                anim.TryGetComponent<Button>(out Button button);
                if (button != null)
                {
                    // make it non-interactable
                    button.interactable = interaction;
                }
            }
            foreach (var anim in m_settingsButtonAnim)
            {
                anim.TryGetComponent<Button>(out Button button);
                if (button != null)
                {
                    // make it non-interactable
                    button.interactable = interaction;
                }
            }
        }

        IEnumerator SwitchingCoroutine()
        {
            // making some dynamic algorithm

            // until and unless these are buttons are animated, make sure they are not interactable
            SetButtonsInteraction(m_defaultButtonsAnim, false);
            SetButtonsInteraction(m_settingsButtonAnim, false);



            // main animation algo




            if (M_SettingState == SettingState.Default)
            {
                m_mainButton.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().GetChild(0).GetComponent<Animator>().CrossFade("Disappear", .1f);
                yield return new WaitForSeconds(m_mainButtonIconDuration);
                m_mainButton.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().GetChild(0).GetComponent<Animator>().CrossFade("Appear", .1f);
                m_mainButton.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().GetChild(0).GetComponent<Image>().sprite = m_settingSprite;

                foreach (var anim in m_defaultButtonsAnim)
                {
                    // get the child logo and animate it as well first
                    if (anim.transform.GetChild(0) != null)
                    {
                        Animator logoAnim = anim.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
                        logoAnim.CrossFade("Disappear", .1f);
                    }

                    yield return new WaitForSeconds(m_durationBetweenLogoAndItsParent);

                    anim.CrossFade("Disappear", .1f);
                    yield return new WaitForSeconds(m_durationBetweenEachButtonAnim);  // you can use custom vars for this waitTime
                }

                yield return new WaitForSeconds(m_durationBetweenMajorSwitching);

                foreach (var anim in m_settingsButtonAnim)
                {
                    anim.CrossFade("Appear", .1f);

                    yield return new WaitForSeconds(m_durationBetweenLogoAndItsParent);

                    // get the child logo and animate it as well first
                    if (anim.transform.GetChild(0) != null)
                    {
                        Animator logoAnim = anim.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
                        logoAnim.CrossFade("Appear", .1f);
                    }

                    yield return new WaitForSeconds(m_durationBetweenEachButtonAnim);

                }

                // Changing the enums now
                M_SettingState = SettingState.Settings;


            }
            else
            {
                m_mainButton.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().GetChild(0).GetComponent<Animator>().CrossFade("Disappear", .1f);
                yield return new WaitForSeconds(m_mainButtonIconDuration);
                m_mainButton.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().GetChild(0).GetComponent<Animator>().CrossFade("Appear", .1f);
                m_mainButton.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>().GetChild(0).GetComponent<Image>().sprite = m_defaultSprite;


                foreach (var anim in m_settingsButtonAnim)
                {
                    // get the child logo and animate it as well first
                    if (anim.transform.GetChild(0) != null)
                    {
                        Animator logoAnim = anim.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
                        logoAnim.CrossFade("Disappear", .1f);
                    }

                    yield return new WaitForSeconds(m_durationBetweenLogoAndItsParent);

                    anim.CrossFade("Disappear", .1f);

                    yield return new WaitForSeconds(m_durationBetweenEachButtonAnim);

                }

                yield return new WaitForSeconds(m_durationBetweenMajorSwitching);

                foreach (var anim in m_defaultButtonsAnim)
                {
                    // get the child logo and animate it as well first

                    anim.CrossFade("Appear", .1f);

                    yield return new WaitForSeconds(m_durationBetweenLogoAndItsParent);

                    if (anim.transform.GetChild(0) != null)
                    {
                        Animator logoAnim = anim.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
                        logoAnim.CrossFade("Appear", .1f);
                    }

                    yield return new WaitForSeconds(m_durationBetweenEachButtonAnim);  // you can use custom vars for this waitTime
                }

                // Changing the enums now
                M_SettingState = SettingState.Default;

            }



            // we need to change the logo of the settings button as well, do it on top of this coroutine
            // when you're about to start the buttons and the logo animations

            // change the interaction in the end of the algo now
            SetButtonsInteraction(m_defaultButtonsAnim, true);
            SetButtonsInteraction(m_settingsButtonAnim, true);
        }
    }
}
