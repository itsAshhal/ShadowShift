using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Fusion.Sockets;
using System;

namespace ShadowShift.Fusion
{
    public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
    {
        private NetworkRunner m_networkRunner;
        public GameMode M_GameMode;
        public int HostSceneIndex = 1;

        private void Start()
        {
            m_networkRunner = GetComponent<NetworkRunner>();
            MakeNewServerGame(M_GameMode);
        }
        async void MakeNewServerGame(GameMode gameMode)
        {
            Debug.Log($"HostMode is {gameMode}");
            var sceneInfo = new NetworkSceneInfo();
            var scene = SceneRef.FromIndex(HostSceneIndex);
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            await m_networkRunner.StartGame
                (
                new StartGameArgs()
                {
                    GameMode = gameMode,
                    SessionName = "ShadowShiftRoom",
                    Scene = scene,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
                });

            
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

        }

        #endregion 
    }
}
