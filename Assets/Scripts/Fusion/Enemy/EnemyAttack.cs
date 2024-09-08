using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace ShadowShift.Fusion
{
    public class EnemyAttack : MonoBehaviour
    {
        public float MoveSpeed;
        public void MoveEnemy(Vector3 target)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, MoveSpeed * Time.deltaTime);
        }
    }


}


