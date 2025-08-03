using Unity.Netcode;
using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts.Projectiles
{
    public class WeaponController : NetworkBehaviour
    {

        [SerializeField]
        private WeaponData weaponData;

        private float cooldown;

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            cooldown -= Time.deltaTime;
            if (cooldown > 0f) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FireProjectile();
            }
        }

        private void FireProjectile()
        {
            var direction = transform.forward;
            var origin = transform.position;

            ServerProjectileManager.Instance.FireWeaponServerRpc(weaponData.Id, origin, direction, NetworkObject.OwnerClientId);

            cooldown = 0.25f;
        }

    }
}