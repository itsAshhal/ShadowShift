using System.Collections;
using System.Collections.Generic;
using ShadowShift.DataModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ShadowShift
{
    public class SplashScreenController : MonoBehaviour
    {
        [SerializeField] ControlType m_ControlType;
        public float DelayTimeFor_Dot;
        public float DelayTimeFor_R;
        public float DelayTimeFor_BottomSpring;
        public float DelayTimeFor_LeftSpring;
        public float DelayTimeFor_Container;
        public float DelayTimeFor_PresentedBy;
        public float DelayTimeFor_RiftStudio;
        public float DelayTimeFor_FadeImage;
        public float StartTimeForFading = 1f;

        public GameObject Dot_Logo;
        public GameObject R_Logo;
        public GameObject BottomSpringLogo;
        public GameObject LeftSpringLogo;
        public GameObject Container;
        public GameObject PresentedBy;
        public GameObject RiftStudio;
        public GameObject FadeImage;

        public Button RestartButton;
        public float ButtonAppearTime = 3f;


        void Awake()
        {
            StartCoroutine(MajorCoroutine());
        }

        IEnumerator MajorCoroutine()
        {
            // turning on FadeImage
            FadeImage.SetActive(true);
            FadeImage.GetComponent<Animator>().CrossFade("FadeOut", .1f);
            yield return new WaitForSeconds(StartTimeForFading);

            Invoke(nameof(Enable_DotTrails), DelayTimeFor_Dot);
            Invoke(nameof(Enable_R), DelayTimeFor_R);
            Invoke(nameof(Enable_BottomSpring), DelayTimeFor_BottomSpring);
            Invoke(nameof(Enable_LeftSpring), DelayTimeFor_LeftSpring);
            Invoke(nameof(Enable_Container), DelayTimeFor_Container);
            Invoke(nameof(Enable_PresentedBy), DelayTimeFor_PresentedBy);
            Invoke(nameof(Enable_RiftStudio), DelayTimeFor_RiftStudio);
            Invoke(nameof(Enable_RestartButton), ButtonAppearTime);

            RestartButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });

            yield return new WaitForSeconds(DelayTimeFor_FadeImage);
            FadeImage.GetComponent<Animator>().CrossFade("FadeIn", .1f);
            yield return new WaitForSeconds(2f);

            // here we need to decide to save the preferred data via JSON format

            if (GameData.FileExists())
            {
                // get the data now
                PlayerData playerData = GameData.LoadData();

                // check if there are controls in it or not
                if (playerData.Controls.Equals("Buttons") || playerData.Controls.Equals("Swipe"))
                {
                    m_ControlType.M_Controls = playerData.Controls == "Buttons" ? ControlType.Controls.Buttons : ControlType.Controls.Swipe;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                }
                else
                {
                    // since no control data is found so we're loading the controls screen here
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); ;


        }

        void Enable_RestartButton() => RestartButton.gameObject.SetActive(true);

        void Enable_DotTrails()
        {
            Dot_Logo.SetActive(true);
            Dot_Logo.GetComponent<TrailRenderer>().enabled = true;
        }
        void Enable_R()
        {
            R_Logo.SetActive(true);
        }
        void Enable_BottomSpring()
        {
            BottomSpringLogo.SetActive(true);
        }
        void Enable_LeftSpring()
        {
            LeftSpringLogo.SetActive(true);
        }

        void Enable_Container()
        {
            Container.GetComponent<Animator>().enabled = true;
        }
        void Enable_PresentedBy()
        {
            PresentedBy.SetActive(true);
        }
        void Enable_RiftStudio()
        {
            RiftStudio.SetActive(true);
        }
    }

}