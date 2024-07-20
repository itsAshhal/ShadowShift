using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShadowShift.UI
{
    public class MainMenuCanvas : MonoBehaviour
    {
        public Image SwipeControlImage;
        public Image ButtonsControlImage;

        public Vector2 m_swipeControlDefaultScale { get; set; }
        public Vector2 m_buttonControlDefaultScale { get; set; }

        private void Awake()
        {
            m_swipeControlDefaultScale = SwipeControlImage.rectTransform.localScale;
            m_buttonControlDefaultScale = ButtonsControlImage.rectTransform.localScale;
        }

        /// <summary>
        /// For Animation, scale either up and down, depending upon the Vector2
        /// </summary>
        /// <param name="image">The image that needs to be animated</param>
        /// <param name="scaleChange">your Vector2 scale value</param>
        /// <param name="duration">How long does it take to change the scale</param>
        public void AnimateImageScale(Image image, Vector2 scaleChange, float duration)
        {
            // Ensure the image is not null
            if (image == null)
            {
                Debug.LogError("Image is null!");
                return;
            }

            // Get the current scale of the image
            Vector2 currentScale = image.rectTransform.localScale;

            // Calculate the target scale
            Vector2 targetScale = scaleChange;

            // Animate the scale change using DOTween
            image.rectTransform.DOScale(targetScale, duration).SetEase(Ease.InOutQuad);
        }

        /// <summary>
        /// For Animation, scale your image back to default
        /// </summary>
        /// <param name="image">The image that needs to be animated</param>
        /// <param name="scale">your Vector2 scale value</param>
        /// <param name="duration">How long does it take to change the scale</param>
        public void AnimateImageDefaultScale(Image image, Vector2 scale, float duration)
        {
            // Calculate the target scale
            Vector2 targetScale = scale;

            // Animate the scale change using DOTween
            image.rectTransform.DOScale(targetScale, duration).SetEase(Ease.InOutQuad);
        }

    }
}
