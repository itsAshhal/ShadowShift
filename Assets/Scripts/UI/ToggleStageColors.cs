using System.Collections;
using UnityEngine;
using ShadowShift.DataModels;
using UnityEngine.UI;

namespace ShadowShift.UI
{
    /// <summary>
    /// Used to efficiently handle the toggling animation and execution of applying the selected color to all the stages as well
    /// </summary>
    public class ToggleStageColors : MonoBehaviour
    {
        public enum ToggleState { On, Off }
        public ToggleState M_ToggleState;

        [SerializeField] Image ToggleSprite;
        [SerializeField] RectTransform m_circleImage;
        [SerializeField] RectTransform m_rightDestination;
        [SerializeField] RectTransform m_leftDestination;
        [SerializeField] float lerpDuration = 0.5f; // Adjust the lerp duration as needed
        [SerializeField] AnimationCurve movementCurve; // AnimationCurve to control smooth movement

        private Color originalColor;

        private void Start()
        {
            originalColor = ToggleSprite.color;
            OnClick_Toggle();
        }

        public void OnClick_Toggle()
        {
            M_ToggleState = M_ToggleState == ToggleState.On ? ToggleState.Off : ToggleState.On;

            StartCoroutine(LerpColorAndMoveImage());
        }

        private IEnumerator LerpColorAndMoveImage()
        {
            float elapsedTime = 0f;
            Color targetColor = M_ToggleState == ToggleState.On ? GameData.SelectedColor : Color.black;
            targetColor.a = 1.0f;

            Vector2 targetPosition = M_ToggleState == ToggleState.On ? m_rightDestination.anchoredPosition : m_leftDestination.anchoredPosition;
            Vector2 initialPosition = m_circleImage.anchoredPosition;

            while (elapsedTime < lerpDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / lerpDuration); // Normalized time (0 to 1)

                // Use the AnimationCurve to get a smoothed 't' value
                float curveValue = movementCurve.Evaluate(t);

                ToggleSprite.color = Color.Lerp(originalColor, targetColor, curveValue);
                m_circleImage.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, curveValue);
                yield return null;
            }

            ToggleSprite.color = targetColor;
            m_circleImage.anchoredPosition = targetPosition;

            GameData.ToggleStageColors = M_ToggleState == ToggleState.On ? true : false;
        }
    }
}
