using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift
{
    public class Stage : MonoBehaviour
    {
        public Transform PlayerSpawnPosition;
        public GameObject[] Enemies;
        public Transform[] EnemySpawnPositions;
        public bool SpawnEnemies = true;

        private void Start()
        {
            if (!SpawnEnemies) return;
            for (int i = 0; i < EnemySpawnPositions.Length; i++)
            {
                Instantiate(Enemies[i], EnemySpawnPositions[i].position, Quaternion.identity);
            }
        }
    }
}
