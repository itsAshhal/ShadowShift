using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadowShift.DataModels;

namespace ShadowShift
{
    public class Stage : MonoBehaviour
    {
        public Transform PlayerSpawnPosition;
        public GameObject[] Enemies;
        public Transform[] EnemySpawnPositions;
        public bool SpawnEnemies = true;
        [Tooltip("So when the user selects different color for his player, he sees his player in this color in the gameplay, we can also apply colors to ths stage by checking this option")]
        public bool ApplyColorToAllTheStageAsWell = false;
        [Tooltip("So we can apply custom colors to all these sprites")]
        public SpriteRenderer[] StageSprites;

        private void Start()
        {
            if (!SpawnEnemies) return;
            for (int i = 0; i < EnemySpawnPositions.Length; i++)
            {
                Instantiate(Enemies[i], EnemySpawnPositions[i].position, Quaternion.identity);
            }

            ApplyColorToAllTheStageAsWell = GameData.ToggleStageColors;

            if (ApplyColorToAllTheStageAsWell == false) return;
            ApplyColorToTheStageAsWell();
        }

        void ApplyColorToTheStageAsWell()
        {
            foreach (var sprite in StageSprites)
            {
                Color color = GameData.SelectedColor;
                color.a = 1.0f;
                sprite.color = color;
            }
        }
    }
}
