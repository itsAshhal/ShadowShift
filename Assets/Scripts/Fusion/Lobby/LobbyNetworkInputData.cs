using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift.Fusion
{
    public struct LobbyNetworkInputData : INetworkInput
    {
        public Vector2 direction;
        public bool isJumping { get; set; }
    }
}
