using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadowShift.DataModels;
using ShadowShift.Player;
//
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

        [Tooltip("Reserved for last stages perhaps")]
        public bool CanDoSlowMo = false;
        [Tooltip("This animators' speed will be effected if the slow mo feature is enabled in this stage")]
        public Animator[] SlowMoAnimators;

        public bool IsLastStage = false;

        private void Start()
        {
            CheckForSlowmoPermission();

            if (SpawnEnemies)
            {
                for (int i = 0; i < EnemySpawnPositions.Length; i++)
                {
                    Instantiate(Enemies[i], EnemySpawnPositions[i].position, Quaternion.identity);
                }
            }


            Debug.Log($"Start method is called and the GameData.Selector color is {GameData.SelectedColor}");

            ApplyColorToAllTheStageAsWell = GameData.ToggleStageColors;

            if (ApplyColorToAllTheStageAsWell == false) return;
            ApplyColorToTheStageAsWell();

        }

        void CheckForSlowmoPermission()
        {
            GameplayController.Instance.M_CanvasManager.SlowMoButton.SetActive(CanDoSlowMo ? true : false);
        }

        void ApplyColorToTheStageAsWell()
        {
            Debug.Log($"Apply color function is called");
            foreach (var sprite in StageSprites)
            {
                Color color = GameData.SelectedColor;
                color.a = 1.0f;
                sprite.color = color;
            }
        }


    }
}
