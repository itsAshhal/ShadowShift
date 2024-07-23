using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift
{
    public class EffectsController : Singleton<EffectsController>
    {
        public ParticleSystem[] DeathEffects;

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
    }
}