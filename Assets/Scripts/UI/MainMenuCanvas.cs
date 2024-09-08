using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShadowShift.Fusion;
using ShadowShift.DataModels;

namespace ShadowShift.UI
{
    /// <summary>
    /// As there are different areas before creating a fully functional room in Fusion,
    /// "IsSession", "IsNickname", "IsMaxPlayers", "None"
    /// </summary>
    public enum FusionRoomCreationState
    {
        IsSession, IsNickname, IsMaxPlayers, None
    }

    public class MainMenuCanvas : MonoBehaviour
    {
        public Image SwipeControlImage;
        public Image ButtonsControlImage;
        public Slider MusicSlider;
        public Slider VolumeSlider;
        public FusionRoomCreationState M_FusionRoomCreationState;
        public MultiplayerRoomCredential M_MultiplayerRoomCredential;

        public Vector2 m_swipeControlDefaultScale { get; set; }
        public Vector2 m_buttonControlDefaultScale { get; set; }

        private void Awake()
        {
            m_swipeControlDefaultScale = SwipeControlImage.rectTransform.localScale;
            m_buttonControlDefaultScale = ButtonsControlImage.rectTransform.localScale;

        }

        private void Start()
        {
            // so at start, we don't have any room creation state
            M_FusionRoomCreationState = FusionRoomCreationState.None;

            
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


        public void OnValueChange_Music(float value)
        {
            // 0 -> 1

            // ascend or descend the volume first
            AudioSource musicAudio = MainMenuController.Instance.MusicAudio;
            musicAudio.volume = value;

            var playerData = GameData.LoadData();
            GameData.SaveData(new PlayerData
            {
                Controls = playerData.Controls,
                Stage = playerData.Stage,
                CameraOrthoSize = playerData.CameraOrthoSize,
                CameraHeight = playerData.CameraHeight,
                MusicValue = value
            });

            Debug.Log($"music value being stored is {value}");
        }

        #region FusionControls

        public void HostGame(string sessionName, string nickName, int maxPlayers)
        {
            FusionConnection.Instance.HostGame(sessionName, nickName, maxPlayers);
        }
        public void JoinGame(string sessionName, string nickName)
        {
            FusionConnection.Instance.JoinGame(sessionName, nickName);
        }


        /// <summary>
        /// So when we start creating multiplayer game, this needs to be called
        /// </summary>
        public void SetFirstFusionRoomCreationState()
        {
            M_FusionRoomCreationState = FusionRoomCreationState.IsSession;
        }

        public void OnClick_HostOption()
        {
            StartCoroutine(SettingUpMultiplayerCoroutine());
        }


        IEnumerator SettingUpMultiplayerCoroutine()
        {
            // Hiding everything first
            M_MultiplayerRoomCredential.HideCoverImage();
            M_MultiplayerRoomCredential.HideInputField();

            SetFirstFusionRoomCreationState();
            M_MultiplayerRoomCredential.DisplayBackground();
            yield return new WaitForSeconds(.5f);
            M_MultiplayerRoomCredential.DisplayInputField();
            yield return new WaitForSeconds(.25f);
            M_MultiplayerRoomCredential.DisplayCoverImage();
            M_MultiplayerRoomCredential.SetDescription(M_MultiplayerRoomCredential.SessionDescription, M_MultiplayerRoomCredential.NormalDescriptionColor);
        }


        public void OnClick_Join()
        {
            this.JoinGame(M_MultiplayerRoomCredential.SessionName, M_MultiplayerRoomCredential.NickName);
        }

        /// <summary>
        /// This next basically symbolizes when creating a fusion room.
        /// So like we keep on clicking next and entering details like
        /// SessionInfo, RoomInfo, SettingUpANickname, Passwords etc
        /// </summary>
        public void OnClick_Next()
        {
            if (M_FusionRoomCreationState == FusionRoomCreationState.IsSession)
            {
                // animate to next part of the room creation state
                if (M_MultiplayerRoomCredential.SessionName != ""
                    &&
                    M_MultiplayerRoomCredential.SessionName.Length > 3
                    )
                {
                    // Now we can proceed further
                    M_FusionRoomCreationState = FusionRoomCreationState.IsNickname;
                    M_MultiplayerRoomCredential.SetupNextCredential(ref this.M_FusionRoomCreationState);

                }
            }
            else if (M_FusionRoomCreationState == FusionRoomCreationState.IsNickname)
            {
                if (M_MultiplayerRoomCredential.NickName != ""
                    &&
                  M_MultiplayerRoomCredential.NickName.Length > 3
                    )
                {
                    // Now we can proceed further
                    M_FusionRoomCreationState = FusionRoomCreationState.IsMaxPlayers;
                    M_MultiplayerRoomCredential.SetupNextCredential(ref this.M_FusionRoomCreationState);

                }
            }

            //else if (M_FusionRoomCreationState != FusionRoomCreationState.IsMaxPlayers)
            else
            {
                // ok so here we need to show a loading indicator and then setup the room finally
                Debug.Log($"MaxPlayers condition satisfied");
                int maxPlayers = int.Parse(M_MultiplayerRoomCredential.MaxPlayers);
                Debug.Log($"MaxPlayers are {maxPlayers}");

                // remember this 1 is only for testing in Unity editor
                if (maxPlayers >= 1 && maxPlayers < 5)
                {
                    M_MultiplayerRoomCredential.DisplayCreateRoom();

                    this.HostGame(M_MultiplayerRoomCredential.SessionName,
                        M_MultiplayerRoomCredential.NickName,
                        maxPlayers
                        );
                }

            }
        }


        #endregion
    }
}
