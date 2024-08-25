using UnityEngine;
using Fusion;

namespace ShadowShift.Fusion
{
    /// <summary>
    /// This script handles the same logic as the normal WanderingSpotLight controller 
    /// but engages over the network as well
    /// </summary>
    public class WanderingSpotLightFusion : NetworkBehaviour
    {
        /*
         * NOTE => This enemy will only walk towards one player at a time amongst all the players currently
         * available in the room.
         * The good news is we are not providing any input to this enemy so don't separate structs for this
         */


        private Transform _currentTarget;
        public Transform LeftWanderingTransform; // wanders left when there is no target to chase
        public Transform RightWanderingTransform;  // wanders right when there is no target to chase
        public float WanderingSpeed = 2f;
        private Transform _currentWanderingTransform;
        public float DistanceReachThreshold = .1f;

        private void Awake()
        {
            Debug.Log($"The enemy has been awakened in the multiplayer");
            _currentWanderingTransform = LeftWanderingTransform;
        }

        public override void FixedUpdateNetwork()
        {
            if (_currentTarget == null)
            {
                // if there is no target to follow we simply need the enemy to keep wandering here and there
                Vector2.MoveTowards(transform.position, _currentWanderingTransform.position, WanderingSpeed * Runner.DeltaTime);

                // check if the distance is reached
                float remainingDistance = Vector2.Distance(transform.position, _currentWanderingTransform.position);
                bool isReached = remainingDistance <= DistanceReachThreshold;

                if (isReached)
                {
                    // change the current wandering transform
                    _currentWanderingTransform = _currentWanderingTransform == LeftWanderingTransform ? RightWanderingTransform : LeftWanderingTransform;
                }
                else
                {
                    return;
                }
            }
            else
            {
                // we need to chase the player
            }
        }


        #region Spotlight Callbacks

        public void OnTriggerEnter_SpotLight(Collider2D collider)
        {
            _currentTarget = collider.transform;
        }
        public void OnTriggerStay_SpotLight(Collider2D collider)
        {

        }
        public void OnTriggerExit_SpotLight(Collider2D collider)
        {
            _currentTarget = null;
        }

        #endregion

    }
}