using UnityEngine;
using Fusion;
using Unity.VisualScripting;


namespace ShadowShift.Fusion
{
    public class ShootingBall : NetworkBehaviour
    {
        public Vector3 ShootTarget;
        public float MoveSpeed = 5.0f;

        [SerializeField] ParticleSystem DestructibleEffect;

        public override void FixedUpdateNetwork()
        {
            if (!HasInputAuthority) return;
            if (!HasStateAuthority) return;  // only the server(Host) can manage these balls

            if (ShootTarget == null) return;


            Vector2 moveDirection = (ShootTarget - transform.position).normalized;
            transform.Translate(moveDirection * MoveSpeed * Runner.DeltaTime);

        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            // we need to destroy the particle when it hits either player or the ground
            if (other.CompareTag("Player") || other.CompareTag("Ground"))
            {
                // also use an RPC for showing a destructive effect as well
                if (Runner.IsServer == false) return;
                RPC_ShowDestructionServer();

                Runner.Despawn(GetComponent<NetworkObject>());
            }
        }


        #region RPC
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPC_ShowDestructionServer()
        {
            RPC_ShowDestructionClient();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]

        public void RPC_ShowDestructionClient()
        {
            // now instantiate a local destructible effect, we have another option for NetworkBehaviour but let's try this
            var newDes = Instantiate(this.DestructibleEffect, this.transform.position, Quaternion.identity);
            Destroy(newDes.gameObject, 1f);
        }

        #endregion
    }
}