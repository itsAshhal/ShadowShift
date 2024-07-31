using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace ShadowShift.Multiplayer
{
    public class PhotonConnectionController : MonoBehaviourPunCallbacks, IConnectionCallbacks
    {
        public static PhotonConnectionController Instance;
        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
        }

        private void Start()
        {
            TryConnecting();
        }

        void TryConnecting()
        {
            if (IsConnectedToInternet()) PhotonNetwork.ConnectUsingSettings();
        }

        private bool IsConnectedToInternet()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }

            return true;
        }

        private string GetConnectionType()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                return "WiFi";
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                return "Mobile Data";
            }
            return "Unknown";
        }


        #region PhotonMethods

        public override void OnConnectedToMaster()
        {
            Debug.Log($"Connected to master server");
        }



        #endregion


    }
}
