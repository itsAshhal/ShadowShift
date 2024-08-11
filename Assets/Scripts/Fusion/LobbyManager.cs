using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ShadowShift.Fusion
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;
        public Transform[] SpawnPositions;
        public Action<Color> OnColorPaletterChange;
        public Action OnChangeVote;
        public Animator ClientMessageContainer;
        public Animator HostLessVotesMessageContainer;
        public TMP_Text TotalVotesText;
        public int GameplaySceneIndex = 4;

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
        }

        private void Start()
        {
            try
            {
                // check if we're connected
                if (FusionConnection.Instance.M_NetworkRunner.IsConnectedToServer) Debug.Log($"Yes we're connected to the Server");

                // check if we're the host (server)
                if (FusionConnection.Instance.M_NetworkRunner.IsServer) Debug.Log($"Yes, we're the Host (Server)");
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"Lobby Manager => {exception.Message}");
            }
        }


        public void OnSelectColorPalette(Image colorImage)
        {
            // get the color from the imae
            Color color = colorImage.color;

            Debug.Log($"Color changed from LobbyManager, color -> {color}");

            // now we need a way to apply this color to the player sprite
            OnColorPaletterChange?.Invoke(color);
        }

        /// <summary>
        /// This game will initiate the actual gameplay where the friends can play together and cover the environment/battle arena.
        /// But only the Host(Server) can inititate this.
        /// </summary>
        public void OnPress_StartGame()
        {
            StartCoroutine(GameStartCoroutine(FusionConnection.Instance.M_NetworkRunner.IsServer));
        }

        IEnumerator GameStartCoroutine(bool isHost)
        {
            switch (isHost)
            {
                case false:
                    ClientMessageContainer.CrossFade("Appear", .1f);
                    yield return new WaitForSeconds(2f);  // so the message will be appeared for mere 2 seconds which is kinda ideal
                    ClientMessageContainer.CrossFade("Disappear", .1f);
                    break;
                case true:
                    //  what should the HOST do....
                    // check first if the current votes are equal to total active players in the room
                    Debug.Log($"Active players are {FusionConnection.Instance.M_NetworkRunner.ActivePlayers.Count()} and TotalVotes are {FusionConnection.Instance.TotalVotes}");

                    //just get total votes from the UI, would be way easier
                    int totalVotes = int.Parse(TotalVotesText.text);

                    if (totalVotes < FusionConnection.Instance.M_NetworkRunner.ActivePlayers.Count())
                    {
                        // there aren't enough votes yet
                        HostLessVotesMessageContainer.CrossFade("Appear", .1f);
                        yield return new WaitForSeconds(2f);
                        HostLessVotesMessageContainer.CrossFade("Disappear", .1f);
                    }
                    else
                    {
                        // here the host needs to change the scene so all other clients can change the scene as well
                        var sceneInfo = new NetworkSceneInfo();
                        var sceneRef = SceneRef.FromIndex(GameplaySceneIndex);
                        sceneInfo.AddSceneRef(sceneRef, UnityEngine.SceneManagement.LoadSceneMode.Single);
                        FusionConnection.Instance.M_NetworkRunner.SceneManager.LoadScene(sceneRef, new NetworkLoadSceneParameters
                        {

                        });
                    }
                    break;
            }
        }

        private int totalVotes = 0; // Server maintains the actual vote count
        /// <summary>
        /// This is used so the player can vote to show that now they're now ready to start the game
        /// </summary>
        public void OnPress_Vote()
        {
            OnChangeVote?.Invoke();
        }





    }
}