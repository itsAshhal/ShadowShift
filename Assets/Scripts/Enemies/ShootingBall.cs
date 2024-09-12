using ShadowShift.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShadowShift.Enemy
{
    public class ShootingBall : MonoBehaviour
    {
        // ok so when this gameObject is instantiated, we need it to move towards the mainPlayer
        public float m_movingSpeed = 2f;
        Vector2 m_targetDirection;

        private void Start()
        {
            // we need to see this target as the position to get direction towards, otherwise the ball would stop when reaching that position
            m_targetDirection = GameplayController.Instance.MainPlayer.transform.position - transform.position;
        }
        private void Update()
        {
            if (GameplayController.Instance.MainPlayer == null) return;

            transform.Translate(m_targetDirection * m_movingSpeed * Time.deltaTime);

            // transform.position = Vector2.MoveTowards(
            //     transform.position,
            //     m_target,
            //     m_movingSpeed * Time.deltaTime
            //     );
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // we need to destroy the the main player as well
            if (collision.CompareTag("Ground") == false && collision.CompareTag("Player") == false) return;
            var part = EffectsController.Instance.ExplosionEffects[Random.Range(0, EffectsController.Instance.ExplosionEffects.Length)];
            var insPart = Instantiate(part, transform.position, Quaternion.identity);
            Destroy(insPart, 1.5f);
            Destroy(gameObject);

        }
    }
}