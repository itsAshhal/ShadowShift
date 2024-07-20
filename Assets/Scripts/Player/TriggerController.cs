using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShadowShift.Player
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TriggerController : MonoBehaviour
    {
        public string TagToEncounter = string.Empty;
        private BoxCollider2D bc;
        public UnityEvent<Collider2D> _OnEnter;
        public UnityEvent<Collider2D> _OnStay;
        public UnityEvent<Collider2D> _OnExit;

        void Awake()
        {
            bc = GetComponent<BoxCollider2D>();
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

    }
}
