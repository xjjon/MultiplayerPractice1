using UnityEngine;
using Unity.Netcode;
using MultiplayerPractice1.Assets.Scripts.Projectiles;
using System.Collections.Generic;

public class ProjectileVisualManager : NetworkBehaviour
{
    public static ProjectileVisualManager Instance { get; private set; }

    public List<WeaponData> WeaponDataList = new List<WeaponData>();

    private Dictionary<int, WeaponData> _weaponDataDictionary = new Dictionary<int, WeaponData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            for (int i = 0; i < WeaponDataList.Count; i++)
            {
                var weapon = WeaponDataList[i];
                if (!_weaponDataDictionary.ContainsKey(weapon.Id))
                {
                    _weaponDataDictionary.Add(weapon.Id, weapon);
                }
            }
        }
    }

    [ClientRpc]
    public void FireLocalVisualClientRPC(int id, Vector3 origin, Vector3 direction)
    {
        WeaponData weapon = GetWeaponDataById(id);
        if (weapon == null) return;

        if (weapon.VisualProjectilePrefab != null)
        {
            GameObject visual = Instantiate(weapon.VisualProjectilePrefab, origin, Quaternion.LookRotation(direction));
            if (visual.TryGetComponent<VisualProjectile>(out var visualProj))
            {
                visualProj.Initialize(origin, direction, weapon.Speed);
            }
        }
    }

    [ClientRpc]
    public void HandleImpactClientRpc(int weaponId, Vector3 position, Vector3 normal)
    {
        // In a real game, look up the weapon data to get the correct impact prefab.
        WeaponData weapon = GetWeaponDataById(weaponId);
        if (weapon == null || weapon.ImpactVfxPrefab == null) return;

        if (weapon.ImpactVfxPrefab != null)
        {
            Instantiate(weapon.ImpactVfxPrefab, position, Quaternion.LookRotation(normal));
        }

        // Play impact SFX  
        // AudioSource.PlayClipAtPoint(weapon.ImpactSfx, position);
    }

    public WeaponData GetWeaponDataById(int id)
    {
        if (_weaponDataDictionary.TryGetValue(id, out var weapon))
        {
            return weapon;
        }
        return null;
    }
}