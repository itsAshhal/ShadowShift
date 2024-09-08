using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShadowShift.Fusion
{
    /// <summary>
    /// Has the control for all the particle effects in the lobby and in the actual gameplay
    /// </summary>
    public class LobbyEffectsController : MonoBehaviour
    {
        public static LobbyEffectsController Instance;

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;
        }

        public ParticleSystem[] EnemyAttacks;
        public bool IsAttackSpawned = false;

        /// <summary>
        /// Throws a particle which can be a random one towards the player
        /// </summary>
        public ParticleSystem ThrowEnemyAttack(Vector3 startingFrom, Vector3 shouldGoTowards)
        {
            if (IsAttackSpawned) return null;
            var randomAttack = EnemyAttacks[Random.Range(0, EnemyAttacks.Length)];

            // make sure to use a custom script on this energy attack or add one and then use RPC to move it towards the player
            var com = randomAttack.AddComponent<EnemyAttack>();
            IsAttackSpawned = true;
            return randomAttack;

        }
    }
}
