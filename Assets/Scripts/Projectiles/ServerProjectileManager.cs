using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts.Projectiles
{
    public class ServerProjectileManager : NetworkBehaviour
    {
        public static ServerProjectileManager Instance { get; private set; }

        // This class is NOT a MonoBehaviour. It's just a lightweight data container.
        private class LogicalProjectile
        {
            public ulong OwnerId;
            public WeaponData Weapon;
            public Vector3 LastPosition;
            public Vector3 CurrentPosition;
            public Vector3 Velocity;
            public float TimeToLive;

            public void Tick(float deltaTime)
            {
                LastPosition = CurrentPosition;
                CurrentPosition += Velocity * deltaTime;
                TimeToLive -= deltaTime;
            }
        }

        private List<LogicalProjectile> _simulatedProjectiles = new List<LogicalProjectile>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public override void OnNetworkSpawn()
        {
            // This script should only run on the server.
            if (!IsServer)
            {
                this.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            // Update all active simulated projectiles
            for (int i = _simulatedProjectiles.Count - 1; i >= 0; i--)
            {
                LogicalProjectile proj = _simulatedProjectiles[i];
                proj.Tick(Time.fixedDeltaTime);

                // Check for collision
                if (Physics.SphereCast(proj.LastPosition, 0.1f, (proj.CurrentPosition - proj.LastPosition).normalized, out RaycastHit hit, Vector3.Distance(proj.LastPosition, proj.CurrentPosition), proj.Weapon.HitMask))
                {
                    // We have a hit!
                    ProcessHit(proj.OwnerId, proj.Weapon, hit.point, hit.normal);
                    _simulatedProjectiles.RemoveAt(i);
                    continue;
                }

                // Check for lifetime expiration
                if (proj.TimeToLive <= 0)
                {
                    _simulatedProjectiles.RemoveAt(i);
                    ProcessHit(proj.OwnerId, proj.Weapon, proj.CurrentPosition, Vector3.up);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void FireWeaponServerRpc(int weaponId, Vector3 origin, Vector3 direction, ulong ownerId)
        {
            // In a real game, you would have a WeaponRegistry to get data from an ID.
            WeaponData weapon = GetWeaponDataById(weaponId);
            if (weapon == null) return;

            switch (weapon.Type)
            {
                case ProjectileType.Raycast:
                    HandleRaycast(ownerId, weapon, origin, direction);
                    break;
                case ProjectileType.Simulated:
                    HandleSimulated(ownerId, weapon, origin, direction);
                    break;
            }
        }

        private void HandleRaycast(ulong ownerId, WeaponData weapon, Vector3 origin, Vector3 direction)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, weapon.Range, weapon.HitMask))
            {
                ProcessHit(ownerId, weapon, hit.point, hit.normal);
            }
        }

        private void HandleSimulated(ulong ownerId, WeaponData weapon, Vector3 origin, Vector3 direction)
        {
            LogicalProjectile proj = new LogicalProjectile
            {
                OwnerId = ownerId,
                Weapon = weapon,
                LastPosition = origin,
                CurrentPosition = origin,
                Velocity = direction.normalized * weapon.Speed,
                TimeToLive = weapon.Lifetime
            };
            _simulatedProjectiles.Add(proj);
            ProjectileVisualManager.Instance.FireLocalVisualClientRPC(weapon.Id, origin, direction);
        }

        private void ProcessHit(ulong ownerId, WeaponData weapon, Vector3 position, Vector3 normal)
        {
            // TODO: Apply damage to the hit object on the server
            // e.g., if (hitObject.TryGetComponent<Health>(out var health)) { health.TakeDamage(weapon.Damage); }

            // Tell the client visual manager to spawn effects on all clients
            ProjectileVisualManager.Instance.HandleImpactClientRpc(weapon.Id, position, normal);
        }

        private WeaponData GetWeaponDataById(int weaponId)
        {
            return ProjectileVisualManager.Instance.GetWeaponDataById(weaponId);
        }
    }
}