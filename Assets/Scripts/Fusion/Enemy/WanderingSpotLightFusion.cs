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
        [Tooltip("When the enemy catches sight of any of the player in the room, it chases it with a little increased speed")]
        public float ChaseSpeed = 5.0f;


        private void Awake()
        {

        }

        private void Start()
        {
            if (HasInputAuthority == false) return;
            Debug.Log($"The enemy has been awakened in the multiplayer");
            _currentWanderingTransform = LeftWanderingTransform;
            _currentTarget = null;
            LeftWanderingTransform.SetParent(null);
            RightWanderingTransform.SetParent(null);
        }

        public override void FixedUpdateNetwork()
        {
            if (HasInputAuthority == false) return;
            Debug.Log($"Enemy has input authority so its moving");
            // Move the enemy
            if (_currentTarget == null) MoveEnemyRandom();
            else MoveEnemyTowardsPlayer();




        }

        void MoveEnemyRandom()
        {
            if (_currentWanderingTransform == null) return;
            transform.position = Vector2.MoveTowards(transform.position, _currentWanderingTransform.position, WanderingSpeed * Runner.DeltaTime);

            // check if the currentTargetTransform has been reached?
            if (Vector2.Distance(transform.position, _currentWanderingTransform.position) <= DistanceReachThreshold)
            {
                _currentWanderingTransform = _currentWanderingTransform == LeftWanderingTransform ? RightWanderingTransform : LeftWanderingTransform;
            }
        }
        void MoveEnemyTowardsPlayer()
        {
            Vector2 targetModifiedPosition = new Vector2(_currentTarget.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetModifiedPosition, ChaseSpeed * Runner.DeltaTime);

        }


        #region Spotlight Callbacks

        public void OnTriggerEnter_SpotLight(Collider2D collider)
        {

            collider.gameObject.TryGetComponent<PlayerControllerFusion>(out PlayerControllerFusion controller);
            Debug.Log($"Major Test, got the Player Fusion Controller");
            if (controller == null) return;
            Debug.Log($"Major Test, got the Player Fusion Controller 2");
            if (controller.M_PlayerHiddenState == PlayerControllerFusion.PlayerHiddenState.Hidden) return;
            Debug.Log($"Major Test, Just entered but the HiddenState is Open");

            _currentTarget = collider.transform;

            // lets try and shoot a ball first
            if (FusionConnection.Instance == null) return;

            // Call the shoot method from the FusionConnection
            FusionConnection.Instance.SpawnShootingParticle(this.transform.position, _currentTarget.position);


            // lets try using an RPC
        }
        public void OnTriggerStay_SpotLight(Collider2D collider)
        {

        }
        public void OnTriggerExit_SpotLight(Collider2D collider)
        {
            _currentTarget = null;

        }

        #endregion

        #region RPC_s


        #endregion

    }
}