using System.Collections.Generic;
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
            public ulong Id;
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

        private ulong _nextProjectileId = 0;

        private List<LogicalProjectile> _simulatedProjectiles = new List<LogicalProjectile>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
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
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            for (int i = _simulatedProjectiles.Count - 1; i >= 0; i--)
            {
                LogicalProjectile projectile = _simulatedProjectiles[i];
                projectile.Tick(Time.fixedDeltaTime);

                // Check for collision
                if (Physics.SphereCast(projectile.LastPosition, 0.1f, (projectile.CurrentPosition - projectile.LastPosition).normalized, out RaycastHit hit, Vector3.Distance(projectile.LastPosition, projectile.CurrentPosition), projectile.Weapon.HitMask))
                {
                    ProcessHit(projectile);
                    _simulatedProjectiles.RemoveAt(i);
                    continue;
                }

                // Check for lifetime expiration
                if (projectile.TimeToLive <= 0)
                {
                    _simulatedProjectiles.RemoveAt(i);
                    ProcessHit(projectile);
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
                // ProcessHit(ownerId, weapon, hit.point, hit.normal);
            }
        }

        private void HandleSimulated(ulong ownerId, WeaponData weapon, Vector3 origin, Vector3 direction)
        {
            LogicalProjectile proj = new LogicalProjectile
            {
                Id = _nextProjectileId++,
                OwnerId = ownerId,
                Weapon = weapon,
                LastPosition = origin,
                CurrentPosition = origin,
                Velocity = direction.normalized * weapon.Speed,
                TimeToLive = weapon.Lifetime
            };
            _simulatedProjectiles.Add(proj);
            ProjectileVisualManager.Instance.FireLocalVisualClientRPC(weapon.Id, proj.Id, origin, direction);
        }

        private void ProcessHit(LogicalProjectile projectile)
        {
            // TODO: Apply damage to the hit object on the server
            // e.g., if (hitObject.TryGetComponent<Health>(out var health)) { health.TakeDamage(weapon.Damage); }

            // Tell the client visual manager to spawn effects on all clients
            ProjectileVisualManager.Instance.HandleImpactClientRpc(projectile.Weapon.Id, projectile.Id, projectile.CurrentPosition, Vector3.up);
        }

        private WeaponData GetWeaponDataById(int weaponId)
        {
            return ProjectileVisualManager.Instance.GetWeaponDataById(weaponId);
        }
    }
}