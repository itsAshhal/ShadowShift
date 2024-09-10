using ShadowShift.DataModels;
using ShadowShift.UI;
using UnityEngine;
using UnityEngine.UI;

namespace shadowShift.UI
{
    public class SavedColorPrefab : MonoBehaviour
    {
        public float Opacity = .25f;
        public void OnClick_Color()
        {
            Color color = GetComponent<Image>().color;
            color.a = Opacity;
            MainMenuController.Instance.ChangeMainMenuBackground(color);

            // also we need to save this color as well
            GameData.SelectedColor = color;
        }
    }

}