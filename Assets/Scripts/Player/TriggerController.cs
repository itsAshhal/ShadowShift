using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShadowShift.Player
{
    public class TriggerController : MonoBehaviour
    {
        public string TagToEncounter = string.Empty;
        private Collider2D bc;
        public UnityEvent<Collider2D> _OnEnter;
        public UnityEvent<Collider2D> _OnStay;
        public UnityEvent<Collider2D> _OnExit;
        public UnityEvent<Collider2D> _OnParticleCollision;

        void Awake()
        {
            bc = GetComponent<Collider2D>();
            bc.isTrigger = true;
        }


        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(TagToEncounter) == false) return;
            _OnEnter?.Invoke(collider);
        }
        void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.CompareTag(TagToEncounter) == false) return;
            _OnStay?.Invoke(collider);
        }
        void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag(TagToEncounter) == false) return;
            _OnExit?.Invoke(collider);
        }

        // Collision with particles

        void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag(TagToEncounter) == false) return;
            _OnParticleCollision?.Invoke(other.GetComponent<Collider2D>());
        }

    }
}
