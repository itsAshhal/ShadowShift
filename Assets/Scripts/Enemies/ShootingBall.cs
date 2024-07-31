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
        Vector2 m_target;

        private void Start()
        {
            m_target = GameplayController.Instance.MainPlayer.transform.position;
        }
        private void Update()
        {
            if (GameplayController.Instance.MainPlayer == null) return;
            transform.position = Vector2.MoveTowards(
                transform.position,
                m_target,
                m_movingSpeed * Time.deltaTime
                );
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