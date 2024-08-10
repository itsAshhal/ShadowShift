using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ShadowShift.UI;
using System.Globalization;

namespace ShadowShift.Fusion
{
    public class MultiplayerRoomCredential : MonoBehaviour
    {
        public TMP_InputField CurrentInputField;
        public TMP_InputField[] AllInputFields;
        public TMP_InputField[] AllInputFieldsJoining;
        public TMP_Text CurrentDescriptionText;
        public Image CoverImage; // contains animation
        public Image BackgroundImage; // contains animation
        public int CurrentInputFieldIndex = 0;
        public string SessionDescription;
        public string NickNameDescription;
        public string MaxPlayersDescription;
        public string CreatingRoomDescription = "Creating your room........";
        public Color CreateRoomTextColor = Color.green;
        public Color NormalDescriptionColor = Color.white;

        public string SessionName;
        public string NickName;
        public string MaxPlayers;

        private void Start()
        {
            CurrentInputFieldIndex = -1;
            SetNextInputField();
        }

        #region ValueChangeCallbakcs

        public void OnValueChange_SessionName(string sessionName)
        {
            this.SessionName = sessionName;
        }
        public void OnValueChange_NickName(string nickName)
        {
            this.NickName = nickName;
        }
        public void OnValueChange_MaxPlayers(string maxPlayers)
        {
            this.MaxPlayers = maxPlayers;
        }

        #endregion

        public void SetNextInputField()
        {
            CurrentInputFieldIndex++;
            CurrentInputField = AllInputFields[CurrentInputFieldIndex];
        }

        public void SetDescription(string description, Color color)
        {
            CurrentDescriptionText.color = color;
            CurrentDescriptionText.text = description;
        }

        public void DisplayBackground()
        {
            BackgroundImage.GetComponent<Animator>().CrossFade("Appear", .1f);
        }
        public void HideBackground()
        {
            BackgroundImage.GetComponent<Animator>().CrossFade("Disappear", .1f);
        }
        public void DisplayCoverImage()
        {
            CoverImage.GetComponent<Animator>().CrossFade("Appear", .1f);
        }
        public void HideCoverImage()
        {
            CoverImage.GetComponent<Animator>().CrossFade("Disappear", .1f);
        }

        public void DisplayInputField()
        {
            CurrentInputField.GetComponent<Animator>().CrossFade("Appear", .1f);
        }


        public void HideInputField()
        {
            CurrentInputField.GetComponent<Animator>().CrossFade("Disappear", .1f);
        }

        public void SetupNextCredential(ref FusionRoomCreationState fusionRoomCreationState)
        {
            StartCoroutine(NextCredentialCoroutine(fusionRoomCreationState));
        }

        IEnumerator NextCredentialCoroutine(FusionRoomCreationState fusionRoomCreationState)
        {
            HideInputField();
            yield return new WaitForSeconds(.3f);
            HideCoverImage();
            yield return new WaitForSeconds(.3f);
            SetNextInputField();
            DisplayInputField();
            yield return new WaitForSeconds(.3f);
            DisplayCoverImage();

            Debug.Log($"Current room creation state {fusionRoomCreationState}");

            if (fusionRoomCreationState == FusionRoomCreationState.IsNickname)
            {
                SetDescription(NickNameDescription, NormalDescriptionColor);
            }
            else if (fusionRoomCreationState == FusionRoomCreationState.IsMaxPlayers)
            {
                SetDescription(MaxPlayersDescription, NormalDescriptionColor);
            }


        }


        public void DisplayCreateRoom()
        {
            StartCoroutine(CreateRoomTextCoroutine());
        }
        IEnumerator CreateRoomTextCoroutine()
        {
            HideCoverImage();
            yield return new WaitForSeconds(.3f);
            SetDescription(CreatingRoomDescription, CreateRoomTextColor);
            DisplayCoverImage();
        }

        public void OnClick_Cross()
        {
            CurrentInputFieldIndex = 0;
            CurrentInputField = AllInputFields[CurrentInputFieldIndex];

            // also set the text of all the input fields to none
            foreach (var field in AllInputFields) field.text = string.Empty;
            foreach (var field in AllInputFieldsJoining) field.text = string.Empty;

            // setting local variables to none as well
            this.MaxPlayers = string.Empty;
            this.SessionName = string.Empty;
            this.NickName = string.Empty;
        }
    }
}