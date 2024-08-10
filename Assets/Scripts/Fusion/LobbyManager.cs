using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift.Fusion
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;
        public Transform[] SpawnPositions;

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
    }
}