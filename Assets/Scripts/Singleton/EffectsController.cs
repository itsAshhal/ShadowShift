using System;
using System.Collections;
using System.Collections.Generic;
using ShadowShift.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShadowShift
{
    public class EffectsController : MonoBehaviour
    {
        public ParticleSystem[] DeathEffects;
        public ParticleSystem ShootingBalls;
        public ParticleSystem[] ExplosionEffects;
        [Tooltip("When the player enters the area of this smoke, he's gonna pushed away to the specific direction")]
        public ParticleSystem PushingSmoke;

        public ParticleSystem[] ThunderParticles;

        private void Start()
        {
            GameplayController.Instance.OnPressSlowMo += this.OnPressSlowMo_Method;
        }

        private void OnPressSlowMo_Method(bool value)
        {
            if (value == false) return;

            // so this will automatically be called when the player applies the slow motion
            // we need to apply the thunder effect
            SpawnParticle(GameplayController.Instance.MainPlayer.transform.position,
            ThunderParticles[Random.Range(0, ThunderParticles.Length)],
            1f
            );
        }



        /// <summary>
        /// Spawn a particle using this method to make it easier
        /// </summary>
        /// <param name="spawnPosition">Your desired spawn position</param>
        /// <param name="part">The particle you wanna spawn</param>
        /// <param name="destroyTime">Destroy your particle after this seconds</param>
        public void SpawnParticle(Vector2 spawnPosition, ParticleSystem part, float destroyTime)
        {
            Destroy(Instantiate(part, spawnPosition, Quaternion.identity), destroyTime);
        }





        public static EffectsController Instance;
        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Destroy(this);
            }
            else Instance = this;
        }
    }
}