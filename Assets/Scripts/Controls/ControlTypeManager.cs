using System.Collections;
using ShadowShift.DataModels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowShift
{
    public class ControlTypeManager : MonoBehaviour
    {
        [SerializeField] ControlType m_controlType;

        public enum DotPositions
        {
            left, middle, right
        }

        public DotPositions M_DotPositions;
        public Animator[] TextAnimators;
        public Animator TextContainer;
        public float AnimationDelay = .1f;

        void Awake()
        {
            foreach (var item in TextAnimators) item.gameObject.SetActive(false);
            TextContainer.gameObject.SetActive(false);
        }

        public void OnPress_Swipe()
        {
            m_controlType.M_Controls = ControlType.Controls.Swipe;
            PressAction();
            StartAnimation(DotPositions.left);
        }

        public void OnPress_Buttons()
        {
            m_controlType.M_Controls = ControlType.Controls.Buttons;
            PressAction();
            StartAnimation(DotPositions.right);
        }

        private void PressAction()
        {
            StartCoroutine(PressActionCoroutine());
            return;

        }

        IEnumerator PressActionCoroutine()
        {
            TextContainer.gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f);
            foreach (var item in TextAnimators)
            {
                item.gameObject.SetActive(true);
                yield return new WaitForSeconds(AnimationDelay);
            }
        }

        public void OnPress_Start()
        {
            StartCoroutine(PressStartCoroutine());
        }
        IEnumerator PressStartCoroutine()
        {
            TextContainer.CrossFade("OnPress", .1f);

            // only save the data when we hit the start button
            GameData.SaveData(new PlayerData
            {
                Controls = m_controlType.M_Controls == ControlType.Controls.Buttons ? "Buttons" : "Swipe"
            });

            // wait a little before initializing the new scene;
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        void Start()
        {
            m_startPosition = Dot.anchoredPosition;
            M_DotPositions = DotPositions.middle;

            // setting up the controls actions

        }

        public RectTransform Dot;
        public RectTransform Left;
        public RectTransform Right;
        public float DesiredDuration = 1f; // Ensure this is a reasonable value
        private float m_timer = 0f;
        private Vector3 m_startPosition;
        private Vector3 m_targetPosition;
        public AnimationCurve M_AnimationCurve;

        void Update()
        {
            AnimateDot();
        }

        public void StartAnimation(DotPositions targetPosition)
        {
            m_timer = 0f;
            m_startPosition = Dot.anchoredPosition;
            M_DotPositions = targetPosition;

            if (M_DotPositions == DotPositions.right)
            {
                m_targetPosition = Right.anchoredPosition;
            }
            else if (M_DotPositions == DotPositions.left)
            {
                m_targetPosition = Left.anchoredPosition;
            }
        }

        public void AnimateDot()
        {
            if (M_DotPositions == DotPositions.middle) return;

            m_timer += Time.deltaTime;
            float percentage = Mathf.Clamp01(m_timer / DesiredDuration);
            Dot.anchoredPosition = Vector3.Lerp(m_startPosition, m_targetPosition, M_AnimationCurve.Evaluate(percentage));

            // Reset to middle after animation is done
            if (percentage >= 1f)
            {
                M_DotPositions = DotPositions.middle;
            }
        }
    }
}
