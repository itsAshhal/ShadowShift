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
using Fusion = Fusion;

namespace ShadowShift.Fusion
{

    public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static FusionConnection Instance;
        private NetworkRunner m_networkRunner;
        public NetworkRunner M_NetworkRunner => m_networkRunner;
        public int HostSceneIndex = 1;
        [SerializeField] NetworkPrefabRef m_playerNetworkPrefab;
        [SerializeField] NetworkPrefabRef m_wanderingEnemySpotlightPrefab;
        [SerializeField] NetworkPrefabRef m_shootingBall;
        public Action OnMoveRight;
        public Action OnMoveLeft;
        public Action OnStop;
        public Action<NetworkRunner, PlayerRef> _OnPlayerJoined;
        public string Nickname;

        [Networked]
        public int TotalVotes { get; set; }


        Dictionary<PlayerRef, NetworkObject> m_spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
        Dictionary<PlayerRef, NetworkObject> m_spawnedWanderingSpotlights = new Dictionary<PlayerRef, NetworkObject>();
        /// <summary>
        /// The wandering spotLight is the enemy that needs to be spawned by the host and don't need to be spawned once they get spawned once
        /// </summary>
        [Networked] bool HasWanderingSpotlightBeingSpawned { get; set; } = false;



        private void Awake()
        {
            // if (Instance != this && Instance != null) Destroy(this);
            // else Instance = this;

            m_networkRunner = gameObject.GetComponent<NetworkRunner>();
            if (m_networkRunner == null)
            {
                m_networkRunner = gameObject.AddComponent<NetworkRunner>();
            }
            Instance = this;
        }


        private void Start()
        {
            m_networkRunner = GetComponent<NetworkRunner>();

            if (m_networkRunner.IsServer) TotalVotes = 0;
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

            this.Nickname = nickName;
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
            this.Nickname = nickName;
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

        /// <summary>
        /// Spawn the shooting particles when the enemy catches the sight of any of the player (which can be your friends)
        /// </summary>
        /// <param name="spawnPos">Spawn position should be your enemy's</param>
        /// <param name="InitialTarget">Should only be the initial target so the particle doesn't follow the player</param>
        public void SpawnShootingParticle(Vector3 spawnPos, Vector3 InitialTarget)
        {
            if (m_networkRunner.IsServer == false) return;

            // Spawn the shooting particle over the network
            var newSpawnedObject = m_networkRunner.Spawn(m_shootingBall, spawnPos, Quaternion.identity, m_networkRunner.LocalPlayer); // input authority will be of the server

            // we need to assign the targets to these balls as well
            newSpawnedObject.TryGetComponent<ShadowShift.Fusion.ShootingBall>(out ShadowShift.Fusion.ShootingBall ball);
            if (ball == null) return;

            // now we have the shooting ball script, assign the targets
            ball.ShootTarget = InitialTarget;

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

                // we need to call this action on each client so we get the latest updates
                _OnPlayerJoined?.Invoke(runner, player);

                int currentSpawnIndex = m_spawnedCharacters.Count;  // so if the spawned players are 0, first the host is spawned at 0 index of the transform
                // then the disctionary gets filled so the next index will be 1 and so on....
                NetworkObject networkPlayerObject = runner.Spawn(m_playerNetworkPrefab, LobbyManager.Instance.SpawnPositions[currentSpawnIndex].position, Quaternion.identity, player);
                m_spawnedCharacters.Add(player, networkPlayerObject);

                // lets try calling it manually on each player when spawning it
                networkPlayerObject.GetComponent<PlayerControllerFusion>().AnimatePlayerViaFusionSync(runner, player);

                Debug.Log($"Desired value {HasWanderingSpotlightBeingSpawned}");

                // spawning some wanderingSpotLights
                if (HasWanderingSpotlightBeingSpawned) return;
                foreach (var tr in LobbyManager.Instance.WanderingEnemySpotlightSpawnPositions)
                {
                    Debug.Log($"Spawning wanderingSpotlight");
                    runner.Spawn(m_wanderingEnemySpotlightPrefab, tr.position, Quaternion.identity, player);
                }

                // once the loop is over, set the flag so the enemy don't get spawned again when somebody else joins
                HasWanderingSpotlightBeingSpawned = true;


            }
            else
            {
                Debug.Log($"Hi there you've just joined a new room as a localClient");
                //_OnPlayerJoined?.Invoke(runner, player);
            }

            // we need to call this action on each client so we get the latest updates
            // lets try calling it manually on each player when spawning it
            //_OnPlayerJoined?.Invoke(runner, player);

        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // when the player leaves we need to despawn the player from the server and also remove it from the dictionary
            if (m_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                // remove it then
                runner.Despawn(networkObject);
                Debug.Log($"The player has left the room");
                m_spawnedCharacters.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (LobbyInputController.Instance == null) return;

            var data = new LobbyNetworkInputData();

            Debug.Log($"OnInput working");

            if (LobbyInputController.Instance.M_DirectionState == LobbyInputController.DirectionState.Right)
            {
                data.direction += Vector2.right;
            }
            else if (LobbyInputController.Instance.M_DirectionState == LobbyInputController.DirectionState.Left)
            {
                data.direction += Vector2.left;
            }
            else
            {
                data.direction = Vector2.zero;
            }


            // check separately for the jump as well
            if (LobbyInputController.Instance.M_JumpingState == LobbyInputController.JumpingState.Yes) data.isJumping = true;
            else data.isJumping = false;

            input.Set(data);  // passing the input to host

        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"Player has shutdown");
            SceneManager.LoadScene("MainMenu");
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log($"connected to server");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.Log($"Player disconnected from the server");
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
