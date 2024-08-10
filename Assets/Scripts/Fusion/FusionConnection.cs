using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Fusion.Sockets;
using System;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;
using ShadowShift.DataModels;
using static Unity.Collections.Unicode;

namespace ShadowShift.Fusion
{
    public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static FusionConnection Instance;
        private NetworkRunner m_networkRunner;
        public NetworkRunner M_NetworkRunner => m_networkRunner;
        public int HostSceneIndex = 1;
        [SerializeField] NetworkPrefabRef m_playerNetworkPrefab;
        [SerializeField] Transform[] m_spawnPositions;

        Dictionary<PlayerRef, NetworkPrefabRef> m_spawnedCharacters = new Dictionary<PlayerRef, NetworkPrefabRef>();

        [Networked]
        public NetworkString<_32> Nickname;


        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
        }


        private void Start()
        {
            m_networkRunner = GetComponent<NetworkRunner>();
        }

        public void HostGame(string sessionName, string nickName, int maxPlayers)
        {
            CreateOnlineGame(GameMode.Host, sessionName, nickName, maxPlayers);
        }
        public void JoinGame(string sessionName, string nickName)
        {
            JoinOnlineGame(GameMode.Client, sessionName, nickName);
        }



        /// <summary>
        /// Creates an online game. Use GameMode.Host if you're making a room and use GameMode.Client if you're joining.
        /// </summary>
        /// <param name="gameMode">What kind of game mode do u want for this game</param>
        async void CreateOnlineGame(GameMode gameMode, string sessionName, string nickName, int maxPlayers)
        {
            Nickname = nickName;

            Debug.Log($"HostMode is {gameMode}");
            var sceneInfo = new NetworkSceneInfo();
            m_networkRunner.ProvideInput = true;
            var scene = SceneRef.FromIndex(HostSceneIndex);
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            await m_networkRunner.StartGame
                (
                new StartGameArgs()
                {
                    GameMode = gameMode,
                    SessionName = sessionName,
                    Scene = scene,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                    PlayerCount = maxPlayers
                }); ;

        }
        async void JoinOnlineGame(GameMode gameMode, string sessionName, string nickName)
        {
            Nickname = nickName;
            var sceneInfo = new NetworkSceneInfo();
            m_networkRunner.ProvideInput = true;
            var scene = SceneRef.FromIndex(HostSceneIndex);
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            var joiningResult = await m_networkRunner.StartGame
            (
                  new StartGameArgs()
                  {
                      GameMode = gameMode,
                      SessionName = sessionName,
                      Scene = scene,
                      SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                  });

            if (joiningResult.Ok) Debug.Log($"Joined successfully");
            else Debug.Log($"Joining failed");
        }

        #region INetworkRunnercallbacks

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {

        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {

        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            // we need to check if we're the server to manage most of the logic
            if (runner.IsServer)
            {
                if (LobbyManager.Instance == null) return;
                Debug.Log($"A new player has joined and the LobbyManager exists");
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {

        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {

        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {

        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log($"connected to server");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {

        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {

        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log($"Couldn't connect to a server");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {

        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {

        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {

        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {

        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {

        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {

        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {

        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log($"Scene load has started");
        }

        #endregion 
    }
}
