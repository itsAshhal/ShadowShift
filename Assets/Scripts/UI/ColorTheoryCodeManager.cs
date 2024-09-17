using System.Collections;
using System.Collections.Generic;
using ShadowShift.DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace shadowShift.UI
{
    /// <summary>
    /// Used to set and then save the new color (Hex code) offered by the user, 
    /// so he can use this color in the feature for this game
    /// </summary>
    public class ColorTheoryCodeManager : MonoBehaviour
    {
        private string currentHexCode = "";
        [Tooltip("When the user correctly types the hex code and tries to save it")]
        public UnityEvent OnHexCodeValidated;
        [Tooltip("When the user doesn't type the hex code correctly")]
        public UnityEvent OnHexCodeNonValidated;
        [Tooltip("The main purpose is to keep changing the button color on each given input by the user, kinda gives a handsome feedback")]
        public Button SaveHexCodeButton;
        public TMP_InputField ColorInputField;

        private void Start()
        {
            SaveHexCodeButton.interactable = false;
        }


        public void OnInputChange_HexCode(string hexCode)
        {
            // Store the input hex code in the currentHexCode variable
            currentHexCode = hexCode;
            if (IsValidHexCode(currentHexCode) == false) return;

            // now change the color of the button
            // Convert hex code to Color object
            Color newColor;
            if (ColorUtility.TryParseHtmlString(currentHexCode, out newColor))
            {
                // Apply the new color to the button
                SaveHexCodeButton.GetComponent<Image>().color = newColor;
                SaveHexCodeButton.interactable = true;
            }
            else
            {
                // Handle the case where TryParseHtmlString fails to parse the color
                SaveHexCodeButton.interactable = false;
                SaveHexCodeButton.GetComponent<Image>().color = Color.black;
            }
        }

        public void OnClick_SaveColor()
        {
            // Validate the hex code
            if (IsValidHexCode(currentHexCode))
            {
                // Save the color or do something with it
                Debug.Log("Valid hex code: " + currentHexCode);
                // Additional code to save or apply the color goes here



                // first check if this color exists or not, we don't need to save duplicate colors
                if (GameData.LoadColorData() != null)
                {
                    foreach (var data in GameData.LoadColorData())
                    {
                        if (data.HexColor == currentHexCode)
                        {
                            OnHexCodeNonValidated?.Invoke();
                            return;
                        }
                    }
                }


                // since the color is validated, we need to save it locally
                GameData.SaveColorData(new ColorData
                {
                    HexColor = currentHexCode
                }, true);

                // once the color code is properly identified, we need to save it and show the loading indicator as well
                OnHexCodeValidated?.Invoke();

                CleanData();
            }
            else
            {
                // Handle the invalid hex code case
                Debug.Log("Invalid hex code entered: " + currentHexCode);
                OnHexCodeNonValidated?.Invoke();
            }
        }
        public void CleanData()
        {
            SaveHexCodeButton.GetComponent<Image>().color = Color.black;
            ColorInputField.text = "";
        }
        private bool IsValidHexCode(string hexCode)
        {
            // Check if the hex code is valid (Optional: You might want to allow #RGB format as well)
            return System.Text.RegularExpressions.Regex.IsMatch(hexCode, "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
        }
    }

}